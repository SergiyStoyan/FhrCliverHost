function show_message(content, title) {
    if (!title)
        title = '&nbsp;';
    var html = '<div title="' + title + '"><p>' + content + '</p></div>';
    var e = $(html);
    $("body").append(e);

    e.on('dialogclose', function (event) {
        e.remove();
    });

    e.dialog({
        resizable: true,
        height: 'auto',
        width: 'auto',
        modal: true,
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        },
        show: {
            effect: "clip",
            duration: 400
        },
        hide: {
            effect: "fade",
            duration: 400
        }
    });
    
    arrange_modal_window(e);
    return e;
}

function show_error(content, title) {
    if (!title)
        title = "ERROR";
    var html = '<div title="' + title + '" style="color:#f00;"><p><span class="ui-icon-alert" style="float:left; margin:0 7px 20px 0;""></span>' + content + '</p></div>';
    var e = $(html);
    $("body").append(e);

    e.on('dialogclose', function (event) {
        e.remove();
    });

    e.dialog({
        resizable: true,
        height: 'auto',
        width: 'auto',
        modal: true,
        buttons: {
            "OK": function () {
                $(this).dialog("close");
            }
        },
        show: {
            effect: "highlight",
            duration: 400
        },
        hide: {
            //effect: "blind",
            effect: "fade",
            duration: 400
        }
    });

    arrange_modal_window(e);
    return e;
}

function arrange_modal_window(e) {
    var h = $(e).parent().height() - $(window).height();
    if (h > 0)
        $(e).height($(e).height() - h - 10);
    var w = $(e).parent().width() - $(window).width();
    if (w > 0)
        $(e).width($(e).width() - w - 10);

    e.dialog({ "position": { my: "center", at: "center", of: window, collision: 'fit' } });
        
    //fixing a bug when the box is stuck with the right edge of the window
    var s = $(e).parent().offset().left + $(e).parent().outerWidth() - $(window).width();
    if (s + 2 > 0) {
        var w = $(e).outerWidth() - $(e).parent().offset().left;
        //alert(w);
        $(e).width(w);
        e.dialog({ "position": { my: "center", at: "center", of: window, collision: 'fit' } });
    }
}

//content_div_id may be not specified
function show_ajax_modal_box(title, buttons, content_div_id) {
    if (content_div_id) {
        if ($('#' + content_div_id).parent().hasClass('ui-dialog-content'))
            $('#' + content_div_id).parent().dialog("destroy");
    }
            
    //if (!title)
    //    title = '&nbsp;';

    var html = '<div><div class="_loading" style="height:100%;width:100%;position:absolute;z-index:10;display:none;"><img src="/Images/ajax-loader.gif" style="display:block;margin:auto;position:relative;top:50%;transform:translateY(-50%);"/></div></div>';
    var e = $(html);
    $("body").append(e);

    var content_e;
    if (content_div_id) {
        content_e = $("#" + content_div_id);
        content_e.addClass("_content");
        content_e.show();
    }
    else {
        content_e = $('<div class="_content"></div>');
    }
    e.append(content_e);
        
    var close = function (event, ui) {
        if (content_div_id)            
            //content_e = $("#" + content_div_id);
            //content_e.hide();
            //$("body").append(content_e);
            e.dialog("close");
        else
            e.remove();
    };
    
    if (!buttons) {
        var buttons = {};
        buttons["Cancel"] = null;
    }
    $.each(buttons, function (name, value) {
        if (!value) 
            buttons[name] = close;
    });    

    e.dialog({
        close: close,
        //open: function (event, ui) {
        //},
        create: function (event, ui) {
            arrange_modal_window(e);
        },
        resizeStop: function (event, ui) {
            //e.dialog({ "position": { my: "center", at: "center", of: window, collision: 'fit' } });
        },
        title: title,
        maxHeight: $(window).height() - 10,
        maxWidth: $(window).width() - 10,
        closeOnEscape: true,
        draggable: true,
        resizable: true,
        height: 'auto',
        width: 'auto',
        modal: true,
        buttons: buttons,
        show: {
            effect: "fade",
            duration: 400
        },
        hide: {
            effect: "fade",
            duration: 400
        }
    });

    e.processing = function (show) {
        if (show || show == undefined) {
            e.find("._loading").show();
            e.find("._content").css('visibility', 'hidden');
        }
        else {
            e.find("._loading").hide();
            e.find("._content").css('visibility', 'visible');
        }
    }
    
    e.title = function (html) {
        if (html == undefined)
            return e.parent().find(".ui-dialog-title").html();
        e.parent().find(".ui-dialog-title").html(html);
    }

    e.content = function (html) {
        if (html == undefined)
            return e.find("._content").html();
        e.find("._content").html(html);
    }

    e.getContentByAjax = function (ajax_config, on_success) {
        if (ajax_config["type"] == undefined)
            ajax_config["type"] = "POST";
        ajax_config["success"] = function (response) {
            e.processing(false);
            on_success(response);
            arrange_modal_window(e);
        };
        ajax_config["error"] = function (xhr, error) {
            e.processing(false);
            show_error(xhr.responseText, error);
        };
        e.processing();
        $.ajax(ajax_config);
    }

    e.close = close;

    return e;
}

function init_table(definition) {
    if (!definition.server.actions_prefix)
        definition.server.actions_prefix = '';
    var default_definition = {
        server: {
            request_path: "!",
            actions_prefix: '',
        },
        table: {
            id_column_id: 0,
        },
        menu: {
            top: {
                new: true,            
            },
            left: {
                delete: true,
            },
            right: {
                details: true,
                edit: true,
            }
        },
        menu_processors: {
            new: {
                text: "New",
                onclick: function () {
                    table.modalBox = definition.show_row_editor(definition.server.request_path + "/Create" + definition.server.actions_prefix, "Create", function () {
                        if (definition.server)
                            table.api().draw();
                        else
                            location.reload();
                    });
                }
            },
            details: {
                text: "Details",
                onclick: function () {
                    if (!table.$('tr.selected').is("tr")) {
                        show_message("No row selected!", "Warning");
                        return false;
                    }
                    var id = table.fnGetData(table.$('tr.selected'))[definition.table.id_column_id];

                    table.modalBox = definition.show_row_editor(definition.server.request_path + "/Details" + definition.server.actions_prefix + "?Id=" + id, "OK");
                },
            },
            edit: {
                text: "Edit",
                onclick: function () {
                    if (!table.$('tr.selected').is("tr")) {
                        show_message("No row selected!", "Warning");
                        return false;
                    }
                    var id = table.fnGetData(table.$('tr.selected'))[definition.table.id_column_id];

                    table.modalBox = definition.show_row_editor(definition.server.request_path + "/Edit" + definition.server.actions_prefix + "?Id=" + id, "Save", function () {
                        if (definition.server)
                            table.api().draw();
                        else
                            location.reload();
                    });
                },
            },
            delete: {
                text: "X",
                onclick: function () {
                    if (!table.$('tr.selected').is("tr")) {
                        show_message("No row selected!", "Warning");
                        return false;
                    }
                    var id = table.fnGetData(table.$('tr.selected'))[definition.table.id_column_id];

                    table.modalBox = definition.show_row_editor(definition.server.request_path + "/Delete" + definition.server.actions_prefix + "?Id=" + id, "Delete", function () {
                        if (definition.server)
                            table.api().draw();
                        else
                            location.reload();
                    });
                },
                style: "color:#f00;",
            },
        },
        datatable: {
            serverSide: true,
            ajax: {
                url: definition.server.request_path + "/TableJson" + definition.server.actions_prefix,
                type: 'POST',
            },
            columnDefs:[
                {
                    visible: false, 
                    targets: 0
                },
            ],
            scrollX: true,
            processing: true,
            language: {
                processing: '<img src="/Images/ajax-loader.gif" style="z-index:1;position:relative"/>'
            },
            rowCallback: definition.on_row_filled,
            paging: true,
            //ordering: false,
            //info: false 
            //"columnDefs": [
            //    { "visible": false, "targets": 0 },
            //],
            //"columns": [
            //  { "visible": false },
            //  null,
            //  null,
            //],
            //"stateSave": true,
            //initComplete: function (settings, json) { alert(json);}
        },
        on_row_clicked: function (row) {
            if (row.hasClass('selected')) {
                row.removeClass('selected');
            }
            else {
                table.$('tr.selected').removeClass('selected');
                row.addClass('selected');
            }
            if (row.hasClass('selected')) {
                var t = row.offset().top;
                var r = table.parents(".dataTables_wrapper");
                if (table.menu.left) {
                    table.menu.left.css('visibility', 'visible');
                    table.menu.left.offset({ 'top': t, 'left': r.offset().left - table.menu.left.outerWidth() });
                    table.menu.left.css("padding-top", row.find('td:first').css("padding-top"));
                    table.menu.left.css("padding-bottom", row.find('td:first').css("padding-bottom"));
                    table.menu.left.innerHeight(row.innerHeight());
                }
                if (table.menu.right) {
                    table.menu.right.css('visibility', 'visible');
                    table.menu.right.offset({ 'top': t, 'left': r.offset().left + r.outerWidth(true) });
                    table.menu.right.css("padding-top", row.find('td:first').css("padding-top"));
                    table.menu.right.css("padding-bottom", row.find('td:first').css("padding-bottom"));
                    table.menu.right.innerHeight(row.innerHeight());
                }
            }
            else {
                if (table.menu.left)
                    table.menu.left.css('visibility', 'hidden');
                if (table.menu.right)
                    table.menu.right.css('visibility', 'hidden');
            }
        },
        on_row_filled: function (row, data, index) {
            h = $(row).html().replace(/(\d{4}\-\d{2}\-\d{2})T(\d{2}\:\d{2}:\d{2})(\.\d+)?/ig, "$1 $2");
            h = h.replace(/(<a\s.*?<\/a\s*>|<img\s.*?>|https?\:\/\/[^\s<>\'\"]*)/ig, function (m) {
                if (m[0] == "<")
                    return m;
                return "<a href=\"" + m + "\">" + m + "</a>";
            });
            $(row).html(h);
        },
        show_row_editor: function (content_url, ok_button_text, on_success) {
            var e;

            var buttons = {};
            if (on_success) {
                buttons[ok_button_text] = function () {
                    if (!e.find("form").valid())
                        return;

                    e.processing();

                    $.ajax({
                        type: e.find("form").attr('method'),
                        url: e.find("form").attr('action'),
                        data: e.find("form").serialize(),
                        success: function (data) {
                            if (!data || data.redirect) {
                                e.remove();
                                on_success();
                                return;
                            }
                            e.processing(false);
                            e.content(data);
                        },
                        error: function (xhr, error) {
                            e.processing(false);
                            show_error(xhr.responseText);
                        }
                    });
                };
                buttons["Cancel"] = function () {
                    e.remove();
                }
            }
            else {
                buttons[ok_button_text] = function () {
                    e.remove();
                }
            }

            e = show_ajax_modal_box(null, buttons);

            e.getContentByAjax(
                {
                    type: "GET",
                    url: content_url,
                },
                function (data) {
                    var title;
                    var r = /\<h2\s*[^>]*\>([^]*)\<\/h2\s*[^>]*\>/mi;
                    var m = r.exec(data);
                    if (m) {
                        title = m[1];
                        data = data.replace(r, "");
                    }
                    e.title(title);

                    e.content(data);
                    e.find("form").find("input[type='submit']").parent().hide();

                    e.find("form").validate();//does not work properly
                    //$.getScript("/Scripts/jquery.validate.js");
                    //$.getScript("/Scripts/jquery.validate.unobtrusive.js");
                    //e.find("form").valid();
                }
            );

            return e;
        },
    };
        
    function overwrite(f, s) {
        for (var i in s) {
            if ($.type(s[i]) != 'object' && $.type(s[i]) != 'array')
                f[i] = s[i];
            else {
                if (f[i] == undefined) {
                    if ($.type(s[i]) == 'object')
                        f[i] = {};
                    else
                        f[i] = [];
                }
                overwrite(f[i], s[i]);
            }
        }
        return f;
    }
    definition = overwrite(default_definition, definition);
    
    for (var i in definition.menu)
        for (var j in definition.menu[i])
            if (definition.menu[i][j] === true)
                definition.menu[i][j] = definition.menu_processors[j];
            else if (definition.menu[i][j] === false)
                delete definition.menu[i][j];

    if (!definition.datatable.serverSide)
        definition.datatable.ajax = false;
    
    var table = $("table:last").dataTable(definition.datatable);

    if (definition.datatable.serverSide) {
        var search_box = table.parent().find(".dataTables_filter").find("input");
        //search_box.keyup(function () {
        search_box.on('keyup', function (event) {
            if (event.keyCode == 13) {
                table.api().search(search_box.val()).draw();
            }
        });
    }

    var menu = {};
    if (!$.isEmptyObject(definition.menu.top)) {
        menu.top = $("<p></p>");
        table.parents(".dataTables_wrapper").before(menu.top);
        for (var i in definition.menu.top) {
            var b = $('<a href="#" class="button" style=' + definition.menu.top[i].style + '>' + definition.menu.top[i].text + '</a>');
            menu.top.append(b);
            b.click(definition.menu.top[i].onclick);
        }
    }
    if (!$.isEmptyObject(definition.menu.right)) {
        menu.right = $('<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>');
        table.append(menu.right);
        for (var i in definition.menu.right) {
            var b = $('<a href="#" class="button" style=' + definition.menu.right[i].style + '>' + definition.menu.right[i].text + '</a>');
            menu.right.append(b);
            b.click(definition.menu.right[i].onclick);
        }
    }
    if (!$.isEmptyObject(definition.menu.left)) {
        menu.left = $('<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>');
        table.append(menu.left);
        for (var i in definition.menu.left) {
            var b = $('<a href="#" class="button" style=' + definition.menu.left[i].style + '>' + definition.menu.left[i].text + '</a>');
            menu.left.append(b);
            b.click(definition.menu.left[i].onclick);
        }
    }

    if (definition.on_row_clicked)
        table.find('tbody').on('click', 'tr', function () { definition.on_row_clicked($(this)); });

    table.on('draw.dt', function () {
        if (menu.left)
            menu.left.css('visibility', 'hidden');
        if (menu.right)
            menu.right.css('visibility', 'hidden');
    });

    table.menu = menu;
    table.definition = definition;

    return table;
}