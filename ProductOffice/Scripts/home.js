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

    var html = '<div><div class="_loading" style="height:100%;width:100%;position:absolute;z-index:10;"><img src="/Images/ajax-loader.gif" style="display:block;margin:auto;position:relative;top:50%;transform:translateY(-50%);"/></div></div>';
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

function show_table_row_editor(content_url, ok_button_text, on_success) {
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
}

//init_table options sample:
var options = {
    server: {
        request_path: "@Request.Path",
        actions_prefix: null,
    },
    id_column_id: 0,
    invisible_column_ids: [],
    rowCallback: "default_rowCallback",
    on_raw_clicked: null,
    menu: {
        above: [
            { text: "New", onclick: "default_new" },
            { text: "Start Group", onclick: function () { }, style: null }
        ],
        left: [
            { text: "X", onclick: "default_delete", style: "default_delete" },
            { text: "Add to Group", onclick: function () { } }
        ],
        right: [
            { text: "Details", onclick: "default_details" },
            { text: "Edit", onclick: "default_edit" }
        ]
    }
};
function init_table(options) {
    var defaults = {
        onclicks: {
            default_new: function () {
                table.modalBox = show_table_row_editor(options.server.request_path + "/Create" + options.server.actions_prefix, "Create", function () {
                    if (options.server)
                        table.api().draw();
                    else
                        location.reload();
                });
            },
            default_delete: function () {
                if (!table.$('tr.selected').is("tr")) {
                    show_message("No row selected!", "Warning");
                    return false;
                }
                var id = table.fnGetData(table.$('tr.selected'))[options.id_column_id];

                table.modalBox = show_table_row_editor(options.server.request_path + "/Delete" + options.server.actions_prefix + "?Id=" + id, "Delete", function () {
                    if (options.server)
                        table.api().draw();
                    else
                        location.reload();
                });
            },
            default_details: function () {
                if (!table.$('tr.selected').is("tr")) {
                    show_message("No row selected!", "Warning");
                    return false;
                }
                var id = table.fnGetData(table.$('tr.selected'))[options.id_column_id];

                table.modalBox = show_table_row_editor(options.server.request_path + "/Details" + options.server.actions_prefix + "?Id=" + id, "OK");
            },
            default_edit: function () {
                if (!table.$('tr.selected').is("tr")) {
                    show_message("No row selected!", "Warning");
                    return false;
                }
                var id = table.fnGetData(table.$('tr.selected'))[options.id_column_id];

                table.modalBox = show_table_row_editor(options.server.request_path + "/Edit" + options.server.actions_prefix + "?Id=" + id, "Save", function () {
                    if (options.server)
                        table.api().draw();
                    else
                        location.reload();
                });
            }
        },
        styles: {
            default_delete: "color:#f00;"
        },
        default_rowCallback: function (row, data, index) {
            console.log(row);
            h = $(row).html().replace(/(\d{4}\-\d{2}\-\d{2})T(\d{2}\:\d{2}:\d{2})(\.\d+)?/ig, "$1 $2");
            h = h.replace(/(<a\s.*?<\/a\s*>|<img\s.*?>|https?\:\/\/[^\s<>\'\"]*)/ig, function (m) {
                if (m[0] == "<")
                    return m;
                return "<a href=\"" + m + "\">" + m + "</a>";
            });
            $(row).html(h);
        },
        default_row_clicked: function (row) {
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
                if (left_menu) {
                    left_menu.css('visibility', 'visible');
                    left_menu.offset({ 'top': t, 'left': r.offset().left - left_menu.outerWidth() });
                    left_menu.css("padding-top", row.find('td:first').css("padding-top"));
                    left_menu.css("padding-bottom", row.find('td:first').css("padding-bottom"));
                    left_menu.innerHeight(row.innerHeight());
                }
                if (right_menu) {
                    right_menu.css('visibility', 'visible');
                    right_menu.offset({ 'top': t, 'left': r.offset().left + r.outerWidth(true) });
                    right_menu.css("padding-top", row.find('td:first').css("padding-top"));
                    right_menu.css("padding-bottom", row.find('td:first').css("padding-bottom"));
                    right_menu.innerHeight(row.innerHeight());
                }
            }
            else {
                if (left_menu)
                    left_menu.css('visibility', 'hidden');
                if (right_menu)
                    right_menu.css('visibility', 'hidden');
            }
        }
    }

    if (!options.server.actions_prefix)
        options.server.actions_prefix = '';

    var definition = {
        "scrollX": true,
        "processing": true,
        "language": {
            "processing": '<img src="/Images/ajax-loader.gif" style="z-index:1;position:relative"/>'
        },
        "paging": true,
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
    };
    if (options.rowCallback) {
        if (defaults[options.rowCallback])
            definition["rowCallback"] = defaults[options.rowCallback];
        else
            definition["rowCallback"] = options.rowCallback;
    }
    if (options.server) {
        definition["serverSide"] = true;
        definition["ajax"] = {
            "url": options.server.request_path + "/TableJson" + options.server.actions_prefix,
            "type": 'POST',
        };
    }
    if (options.invisible_column_ids) {
        definition["columnDefs"] = Array();
        for (var i = options.invisible_column_ids.length - 1; i >= 0; i--)
            definition["columnDefs"].push({ "visible": false, "targets": options.invisible_column_ids[i] });
    }

    var table = $("table:last").dataTable(definition);

    //table.columns[id_column_id].visible(show_id_column);

    if (options.server) {
        var search_box = table.parent().find(".dataTables_filter").find("input");
        //search_box.keyup(function () {
        search_box.on('keyup', function (event) {
            if (event.keyCode == 13) {
                table.api().search(search_box.val()).draw();
            }
        });
    }

    var menus = {};
    if (options.menu.above && options.menu.above.length) {
        var above_menu = $("<p></p>");
        table.parents(".dataTables_wrapper").before(above_menu);
        for (var i in options.menu.above) {
            var style = defaults.styles[options.menu.above[i].style];
            if (!style)
                style = options.menu.above[i].style;
            var b = $('<a href="#" class="button" style=' + style + '>' + options.menu.above[i].text + '</a>');
            above_menu.append(b);
            if (defaults.onclicks[options.menu.above[i].onclick])
                b.click(defaults.onclicks[options.menu.above[i].onclick]);
            else
                b.click(options.menu.above[i].onclick);
        }
    }
    var right_menu;
    if (options.menu.right && options.menu.right.length) {
        var right_menu = $('<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>');
        $("body").append(right_menu);
        for (var i in options.menu.right) {
            var style = defaults.styles[options.menu.right[i].style];
            if (!style)
                style = options.menu.right[i].style;
            var b = $('<a href="#" class="button" style=' + style + '>' + options.menu.right[i].text + '</a>');
            right_menu.append(b);
            if (defaults.onclicks[options.menu.right[i].onclick])
                b.click(defaults.onclicks[options.menu.right[i].onclick]);
            else
                b.click(options.menu.right[i].onclick);
        }
    }
    var left_menu;
    if (options.menu.left && options.menu.left.length) {
        left_menu = $('<div class="table_floating_menu" style="visibility: hidden; position: absolute;"></div>');
        $("body").append(left_menu);
        for (var i in options.menu.left) {
            var style = defaults.styles[options.menu.left[i].style];
            if (!style)
                style = options.menu.left[i].style;
            var b = $('<a href="#" class="button" style=' + style + '>' + options.menu.left[i].text + '</a>');
            left_menu.append(b);
            if (defaults.onclicks[options.menu.left[i].onclick])
                b.click(defaults.onclicks[options.menu.left[i].onclick]);
            else
                b.click(options.menu.left[i].onclick);
        }
    }

    table.find('tbody').on('click', 'tr', function () {
        defaults.default_row_clicked($(this));
        if (options.on_raw_clicked)
            options.on_raw_clicked();
    });

    table.on('draw.dt', function () {
        if (left_menu)
            left_menu.css('visibility', 'hidden');
        if (right_menu)
            right_menu.css('visibility', 'hidden');
    });

    return table;
}