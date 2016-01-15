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
    
    arrange_dialog_window(e);
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

    arrange_dialog_window(e);
    return e;
}

function arrange_dialog_window(e) {
    if (!$(e).dialog("isOpen"))
        return;
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
        $(e).width(w);
        e.dialog({ "position": { my: "center", at: "center", of: window, collision: 'fit' } });
    }
}

function show_dialog(definition) {
    var definition_ = {
        content_div_id: false,
        adjust: true,
        background: null,
        dialog:{
            //close: definition_.on_close,
            //open: function (event, ui) {
            //},
            create: function (event, ui) {
                var e = definition_._e;
                arrange_dialog_window(e);
            },
            resizeStop: function (event, ui) {
                var e = definition_._e;
                //e.dialog({ "position": { my: "center", at: "center", of: window, collision: 'fit' } });
            },
            title: '',
            maxHeight: $(window).height() - 10,
            maxWidth: $(window).width() - 10,
            closeOnEscape: true,
            draggable: true,
            resizable: true,
            height: 'auto',
            width: 'auto',
            modal: true,
            buttons: null,
            show: {
                effect: "fade",
                duration: 400
            },
            hide: {
                effect: "fade",
                duration: 400
            },
            closeOnEscape: true,
        },
        on_close: function (event, ui) {
            var e = definition_._e;
            if (e.definition.content_div_id)
                //content_e = $("#" + content_div_id);
                //content_e.hide();
                //$("body").append(content_e);
                e.dialog("close");
            else
                e.remove();
        },
        _e:"!!!",
    };
    if (!definition)
        return definition_;

    function merge(f, s, overwrite) {
        for (var i in s) {
            if ($.type(s[i]) != 'object' && $.type(s[i]) != 'array') {
                if (overwrite || f[i] == undefined)
                    f[i] = s[i];
            }
            else {
                if (f[i] == undefined) {
                    if ($.type(s[i]) == 'object')
                        f[i] = {};
                    else
                        f[i] = [];
                }
                merge(f[i], s[i], overwrite);
            }
        }
        return f;
    }
    //be sure that the output definition has come as init_table parameter! 
    //If using an internal object as definition, it will bring to buggy confusing when several dialogs are on the same page
    definition = merge(definition, definition_);

    if (!definition.dialog.close)
        definition.dialog.close = definition.on_close;
    
    if (definition.content_div_id) {
        if ($('#' + definition.content_div_id).parent().hasClass('ui-dialog-content'))
            $('#' + definition.content_div_id).parent().dialog("destroy");
    }

    var html = '<div><div class="_loading" style="height:100%;width:100%;position:absolute;z-index:10;display:none;"><img src="/Images/ajax-loader.gif" style="display:block;margin:auto;position:relative;top:50%;transform:translateY(-50%);"/></div></div>';
    var e = $(html);
    e.definition = definition;
    //actually defintion's functions are using the object where they are defined, so dialog is to be passed there!
    definition_._e = e;
    $("body").append(e);

    var content_e;
    if (definition.content_div_id) {
        content_e = $("#" + definition.content_div_id);
        content_e.addClass("_content");
        content_e.show();
    }
    else {
        content_e = $('<div class="_content"></div>');
    }
    e.append(content_e);
            
    e.dialog(definition.dialog);

    if (definition.background) {
        e.parent().find('*[class^="ui-resizable-handle"]').css('background-color', definition.background);
        e.parent().find('.ui-widget-content').css('background-color', definition.background);
        e.parent().css('background', definition.background);
    }

    e.show_processing = function (show) {
        if (show || show === undefined) {
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
            e.show_processing(false);
            on_success(response);
            if (e.definition.adjust)
                arrange_dialog_window(e);
        };
        ajax_config["error"] = function (xhr, error) {
            e.show_processing(false);
            show_error(xhr.responseText, error);
        };
        e.show_processing();
        $.ajax(ajax_config);
    }

    e.is_open = function(){
        return $(e).dialog("isOpen");
    }

    e.close = definition.on_close;
    e.definition = definition;

    return e;
}

function init_table(definition) {
    var definition_ = {
        on_row_clicked: function (event) {
            var row = $($(event.target).parents('tr'));
            var table = definition_._table;
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
                    if (table.menu.left.hasClass('outside'))
                        table.menu.left.offset({ 'top': t, 'left': r.offset().left - table.menu.left.outerWidth() });
                    else
                        table.menu.left.offset({ 'top': t, 'left': r.offset().left });
                    table.menu.left.css("padding-top", row.find('td:first').css("padding-top"));
                    table.menu.left.css("padding-bottom", row.find('td:first').css("padding-bottom"));
                    table.menu.left.innerHeight(row.innerHeight());
                }
                if (table.menu.right) {
                    table.menu.right.css('visibility', 'visible');
                    if (table.menu.right.hasClass('outside'))
                        table.menu.right.offset({ 'top': t, 'left': r.offset().left + r.outerWidth(true) });
                    else
                        table.menu.right.offset({ 'top': t, 'left': r.offset().left + r.outerWidth(true) - table.menu.right.outerWidth() });
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
        on_row_filling: function (row, cs, index) {
            for (i in cs) {
                if ($.type(cs[i]) == 'string') {
                    var h = cs[i].replace(/(\d{4}\-\d{2}\-\d{2})T(\d{2}\:\d{2}:\d{2})(\.\d+)?/ig, "$1 $2");
                    h = h.replace(/(<a\s.*?<\/a\s*>|<img\s.*?>|https?\:\/\/[^\s<>\'\"]*)/ig, function (m) {
                        if (m[0] == "<")
                            return m;
                        return "<a href=\"" + m + "\">" + m + "</a>";
                    });
                    cs[i] = h;
                }
            }
            var table = definition_._table;
            if (!table.api)//when filling table by html, this function is called before _table is set. 
                table = $(row).parents('table').dataTable();
            table.api().row(index).data(cs);
        },
        show_row_editor: function (content_url, ok_button_text, on_success) {
            var e;

            var buttons = {};
            if (on_success) {
                buttons[ok_button_text] = function () {
                    if (!e.find("form").valid())
                        return;

                    e.show_processing();

                    $.ajax({
                        type: e.find("form").attr('method'),
                        url: e.find("form").attr('action'),
                        data: e.find("form").serialize(),
                        success: function (data) {
                            if (!data || data.redirect) {
                                e.close();
                                on_success();
                                return;
                            }
                            e.show_processing(false);
                            e.content(data);
                        },
                        error: function (xhr, error) {
                            e.show_processing(false);
                            show_error(xhr.responseText);
                        }
                    });
                };
                buttons["Cancel"] = function () {
                    e.close();
                }
            }
            else {
                buttons[ok_button_text] = function () {
                    e.close();
                }
            }

            e = show_dialog({ dialog: { buttons: buttons } });

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
        table_id: null,
        server: {
            request_path: "!!!",
            actions_prefix: '',
        },
        key_column_ids2name: {
            0: "id",
        },
        menu: {
            top: {
                new: true,
            },
            left: {
                delete: true,
                details: true,
                edit: true,
            },
            right: {
            },
            _templates: {
                new: {
                    text: "New",
                    onclick: function () {
                        var table = definition_._table;
                        table.modalBox = table.definition.show_row_editor(table.definition.server.request_path + "/Create" + table.definition.server.actions_prefix, "Create", function () {
                            if (table.definition.server)
                                table.api().draw(false);
                            else
                                location.reload();
                        });
                    },
                    style: null,
                    class: null,
                },
                details: {
                    text: "Details",
                    onclick: function () {
                        var table = definition_._table;
                        if (!table.$('tr.selected').is("tr")) {
                            show_message("No row selected!", "Warning");
                            return false;
                        }
                        var parameters = [];
                        for (i in table.definition.key_column_ids2name)
                            parameters.push(table.definition.key_column_ids2name[i] + "=" + table.fnGetData(table.$('tr.selected'))[i]);

                        table.modalBox = table.definition.show_row_editor(table.definition.server.request_path + "/Details" + table.definition.server.actions_prefix + "?" + parameters.join('&'), "OK");
                    },
                    style: null,
                },
                edit: {
                    text: "Edit",
                    onclick: function () {
                        var table = definition_._table;
                        if (!table.$('tr.selected').is("tr")) {
                            show_message("No row selected!", "Warning");
                            return false;
                        }
                        var parameters = [];
                        for (i in table.definition.key_column_ids2name)
                            parameters.push(table.definition.key_column_ids2name[i] + "=" + table.fnGetData(table.$('tr.selected'))[i]);

                        table.modalBox = table.definition.show_row_editor(table.definition.server.request_path + "/Edit" + table.definition.server.actions_prefix + "?" + parameters.join('&'), "Save", function () {
                            if (table.definition.server)
                                table.api().draw(false);
                            else
                                location.reload();
                        });
                    },
                    style: null,
                },
                delete: {
                    text: "Delete",
                    onclick: function () {
                        var table = definition_._table;
                        if (!table.$('tr.selected').is("tr")) {
                            show_message("No row selected!", "Warning");
                            return false;
                        }
                        var parameters = [];
                        for (i in table.definition.key_column_ids2name)
                            parameters.push(table.definition.key_column_ids2name[i] + "=" + table.fnGetData(table.$('tr.selected'))[i]);

                        table.modalBox = table.definition.show_row_editor(table.definition.server.request_path + "/Delete" + table.definition.server.actions_prefix + "?" + parameters.join('&'), "Delete", function () {
                            if (table.definition.server)
                                table.api().draw(false);
                            else
                                location.reload();
                        });
                    },
                    style: "color:#f00;",
                },
            },
        },
        datatable: {
            serverSide: true,
            //ajax: {
            //    url: null,
            //    type: 'POST',
            //},
            columnDefs: [
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
            createdRow: null,
            //rowCallback: definition.on_row_filling,
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
        _table: "!!!",
        _merge: function merge(f, s, overwrite) {
            for (var i in s) {
                if ($.type(s[i]) != 'object' && $.type(s[i]) != 'array') {
                    if (overwrite || f[i] == undefined)
                        f[i] = s[i];
                }
                else {
                    if (f[i] == undefined) {
                        if ($.type(s[i]) == 'object')
                            f[i] = {};
                        else
                            f[i] = [];
                    }
                    merge(f[i], s[i], overwrite);
                }
            }
            return f;
        },
    };
    if (!definition)
        return definition_;

    //be sure that the output definition has come as init_table parameter! 
    //If using an internal object as definition, it will bring to buggy confusing when several datatables on the same page
    var definition = definition_._merge(definition, definition_);

    if (!definition.server.actions_prefix)
        definition.server.actions_prefix = '';
    //if (!definition.datatable.ajax.url)
    //    definition.datatable.ajax.url = definition.server.request_path + "/TableJson" + definition.server.actions_prefix;    
    if (!definition.datatable.ajax){
        definition.datatable.ajax = function (data, callback, settings) {
            $.ajax({
                type: 'POST',
                url: definition.server.request_path + "/TableJson" + definition.server.actions_prefix,
                data: data,
                success: function (data) {
                    if ($.type(data) == 'string') {
                        show_error(data);
                        data = { draw: 0, recordsTotal: 0, recordsFiltered: 0, data: [] };
                    }
                    callback(data);
                },
                error: function (xhr, error) {
                    console.log(error, xhr);
                    show_error(xhr.responseText);
                }
            });
        }
    }
    if (!definition.datatable.createdRow)
        definition.datatable.createdRow = definition.on_row_filling;

    for (var i in definition.menu)
        for (var j in definition.menu[i])
            if (definition.menu[i][j] === true)
                definition.menu[i][j] = definition.menu._templates[j];
            else if (definition.menu[i][j] === false)
                delete definition.menu[i][j];

    if (!definition.datatable.serverSide)
        definition.datatable.ajax = false;

    var table;
    if (definition.table_id)
        table = $("#" + definition.table_id).dataTable(definition.datatable);
    else
        table = $("table:last").dataTable(definition.datatable);
    //actually defintion's functions are using the object where they are defined, so table is to be passed there!
    definition_._table = table;
    //also some redefined functions may come from the customer's defintion, so table is to be passed there as well.
    definition._table = table;

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
    var fill_menu = function (me, md) {
        for (var i in md) {
            var b = $('<a href="#" name=' + i + ' class="' + (md[i].class ? md[i].class : 'button') + '" style="' + (md[i].style ? md[i].style : '') + '">' + md[i].text + '</a>');
            me.append(b);
            b.click(md[i].onclick);
        }
    }
    if (!$.isEmptyObject(definition.menu.top)) {
        menu.top = $('<p class="table_fixed_menu"></p>');
        table.parents(".dataTables_wrapper").before(menu.top);
        fill_menu(menu.top, definition.menu.top);
    }
    if (!$.isEmptyObject(definition.menu.right)) {
        menu.right = $('<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>');
        table.parents(".dataTables_wrapper").after(menu.right);
        fill_menu(menu.right, definition.menu.right);
    }
    if (!$.isEmptyObject(definition.menu.left)) {
        menu.left = $('<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>');
        table.parents(".dataTables_wrapper").after(menu.left);
        fill_menu(menu.left, definition.menu.left);
    }

    if (definition.on_row_clicked)
        table.find('tbody').on('click', 'tr', definition.on_row_clicked);

    table.on('draw.dt', function () {
        if (menu.left)
            menu.left.css('visibility', 'hidden');
        if (menu.right)
            menu.right.css('visibility', 'hidden');
    });

    table.show_processing = function (show) {
        var e = $('.dataTables_processing', table.closest('.dataTables_wrapper'));
        if (show || show === undefined)
            e.show();
        else
            e.hide();
    }

    table.menu = menu;
    table.definition = definition;
    
    return table;
}
