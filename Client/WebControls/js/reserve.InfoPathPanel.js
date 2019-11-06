initInfoPathPanel = function (jq_src, jqui, canApprove) {
    jQuery(function () {
        jQuery("td.DocStatus a:contains('True')")
        .AddClass("ui-icon-circle-check")
        .hover(
            function () {
                jQuery(this).AddClass("ui-icon-circle-close")
            },
            function () {
                jQuery(this).RemoveClass("ui-icon-circle-close")
            });
        jQuery("td.DocStatus a:contains('False')")
        .AddClass("ui-icon-circle-minus")
        .hover(
            function () {
                jQuery(this).AddClass("ui-icon-circle-check")
            },
            function () {
                jQuery(this).RemoveClass("ui-icon-circle-check")
            });
    });
}