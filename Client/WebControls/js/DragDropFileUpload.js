
initDragDropFileUpload = function (cid, zonecid) {
    var i = 0;
    var s = [];
    var showModal = function () {

        var d = jQuery("<div title='Form upload'><div id='upStat'></div></div>")
            .dialog({
                position: { my: "center top", at: "center top+20" },
                width: "75%",
                height: "auto",
                modal: true,
                buttons: {
                    'Print': function () {
                        newWin = window.open();
                        newWin.document.write(jQuery("#upStat").html());
                        newWin.location.reload();
                        newWin.focus();
                        newWin.print();
                        newWin.close();
                    },
                    Ok: function () {
                        jQuery(this).dialog("close");
                        window.location.href = window.location.href;
                    }
                }
            });


        // some text to let the user know something is actually happening
        jQuery("#upStat")
            .html(jQuery("#upStat")
                .html() + "Uploading ");

        // initial presentation of this dialog should have the buttons (ok & print)
        // disabled while items are being uploaded
        jQuery(".ui-dialog-buttonpane button").addClass("ui-state-disabled");
        jQuery(".ui-dialog-buttonpane button").prop("disabled", true);

    };

    jQuery("#" + cid).fileupload({
        dataType: "json",
        dropZone: jQuery("#" + zonecid),
        autoupload: false,
        sequentialUploads: false,
        drop: function (e, data) {
            if (i == 0) {
                i += data.files.length;
                showModal();
            }
        },
        add: function (e, data) {
            if (i == 0) {
                i += data.files.length;
                showModal();
            }

            var filenames = [];
            for (file in data.files)
                filenames.push(data.files[file].name);

            jQuery("#upStat").html(
                jQuery("#upStat").html() + filenames.join(", "));

            var jqXhr = data
                .submit()
                .success(function (result, textStatus, jqXHR) {
                    if (result["error"] != undefined) {
                        messageItems = eval(result["error"]);
                        for (j in messageItems)
                            if (/[\w]+/i.test(messageItems[j]))
                                s.push(messageItems[j]);
                    }
                })
                .error(function (jqXHR, textStatus, errorThrown) { s.push(jqXHR.responseText); })
                .always(function (result, textStatus, jqXHR) {
                    i--;

                    if (i == 0) {
                        if (s.length == 0)
                            s.push("All Submissions Successful!");

                        markup = "<div title='Form Submission Status'><ol>";
                        for (j in s)
                            if (/[\w]+/i.test(s[j]))
                                markup +=
                                    "<li>" +
                                    s[j].replace(/[\n\r]+/ig, "<br>") +
                                    "</li>";
                        markup += "</ol></div>";

                        jQuery("#upStat").html(
                            jQuery("#upStat").html() + markup);
                    }
                    s = [];

                    jQuery(".ui-dialog-buttonpane button").removeClass("ui-state-disabled");
                    jQuery(".ui-dialog-buttonpane button").prop("disabled", false);

                });
        }
    });
}