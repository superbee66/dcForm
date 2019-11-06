//-------------------------------------------------------------------------------------------------------------------->
//  Json Serializations
//-------------------------------------------------------------------------------------------------------------------->
function openForm(url) {
    window.location = url;
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

function getJsonFileName() {

    var loc = window.location.search;
    var queryString = loc.substring(1);

    var param = new Array();
    var arr = queryString.split('&');

    for (var i = 0; i < arr.length; i++) {
        var index = arr[i].indexOf('=');
        var key = arr[i].substring(0, index);
        var val = arr[i].substring(index + 1);

        param[key] = val;
    }

    return 'Data/' + param['id'] + '.json'; ;
}

//-------------------------------------------------------------------------------------------------------------------->
//  Multiple Exclusive Methods
//-------------------------------------------------------------------------------------------------------------------->

function cbExclusive(obj) {
    //alert(" I am inside checkbox check");
    //cbOpt_0__Item2_1
    var id = $(obj).attr("id");
    var hiddenID = id.substring(2, id.length - 2);
    var name = $(obj).attr("name");
    $('.' + name).val('');
    //$('#' + output).val() = '';
    if (obj.checked) {
        $('input[name=' + name + ']').each(function () { $('input[name=' + name + ']').attr('checked', false); });
        $('#' + id).attr('checked', true);
        $('.' + name).val($(obj).val());
        $("#" + hiddenID).val($(obj).val());
        //$('#' + output).val() = $(obj).val();
    }
}

function labelExclusive(el) {
    var id = $(el).attr('for')
    var obj = document.getElementById(id);
    cbExclusive(obj);
}

// content is the data you'll write to file<br/>
// filename is the filename<br/>
// what I did is use iFrame as a buffer, fill it up with text
//Invoke the function:
//save_content_to_file("Hello", "C:\\test");
function save_content_to_file(content, filename) {
    var dlg = false;
    with (document) {
        ir = createElement('iframe');
        ir.id = 'ifr';
        ir.location = 'about.blank';
        ir.style.display = 'none';
        body.appendChild(ir);
        with (getElementById('ifr').contentWindow.document) {
            open("text/plain", "replace");
            charset = "utf-8";
            write(content);
            close();
            document.charset = "utf-8";
            dlg = execCommand('SaveAs', false, filename + '.txt');
        }
        body.removeChild(ir);
    }
    return dlg;
}

function checkMultiSelect(obj) {

    var objResults = '';
    var name = $(obj).attr('name');
    $('input[name=' + name + ']').each(function () { if ($(this).is(':checked')) { objResults += $(this).val() + ','; } });
    objResults = objResults.substring(0, objResults.length - 1);
    $('.' + name).val(objResults);
}


function ddlExclusive(obj) {

    var str = '';
    var name = $(obj).attr("name");

    $('select[name=' + name + ']').each(function () {
        var id = $(this).attr("id");
        str += id.replace('ddl', '') + ': ' + $('select[id=' + id + '] option:selected').text() + ", ";
    });

    $('.' + name).val(str.substring(0, str.length - 2));
}

//-------------------------------------------------------------------------------------------------------------------->

function populateCheckBoxes() {

    $('input[type=hidden]').each(function () {

        var objValue = $(this).val();
        var objName = $(this).attr("name");
        var keyName = objName.replace('[', '_').replace(']', '_').replace('.', '_');

        $('input:checkbox[name=cb' + keyName + ']').each(function () {
            if ($(this).val() == objValue) { $(this).attr('checked', true); }
        });
    });
}

function populateCheckBoxSplits(obj) {

    var services = $('#' + obj).val();

    var arr = services.split(',');
    for (var i = 0; i < arr.length; i++) {
        var objValue = arr[i];
        $('input:checkbox[name=cb' + obj + ']').each(function () {
            if ($(this).val() == objValue) { $(this).attr('checked', true); }
        });
    }
}


/*//-------------------------------------------------------------------------------------------------------------------->
Input Restrictions allows to control the length of the field dynamically by the size of the container.
//-------------------------------------------------------------------------------------------------------------------->*/
function objMaxWidth(obj) {

    var myText = $(obj).val();
    $('#objTextBox').text(myText);
    $('#objTextArea').html('');

    var objSize = $(obj).width() - 20;
    var curWidth = $('#objTextBox').width();

    $('#objDescription').text(curWidth.toString() + ' of ' + objSize.toString());

    if (curWidth > objSize) {
        var objLength = $(obj).val().length;
        $(obj).val(myText.substring(0, objLength - 1));
    }
}

function objMaxHeight(obj) {

    var myText = $(obj).val();
    var output = myText.replace(/\n/g, "<br />");
    $('#objTextBox').text('');

    var objWidth = $(obj).width();
    var objHeight = $(obj).height();

    $('#objTextArea').attr('style', 'max-width: ' + objWidth + 'px;').html(output);

    var curWidth = $('#objTextArea').width();
    var curHeight = $('#objTextArea').height();

    $('#objDescription').text('width: ' + curWidth + ' of ' + objWidth + ' height: ' + curHeight + ' of ' + objHeight);

    var i = 0;
    if (curHeight > (objHeight - 2)) {

        i++;
        var objLength = $(obj).text().length;
        var curLength = $('#objTextArea').html().length;

        $(obj).val(myText.substring(0, objLength - i));
        $('#objTextArea').html(output.substring(0, curLength - i));

        $('#objDescription').text('width: ' + curWidth + ' of ' + objWidth + ' height: ' + curHeight + ' of ' + objHeight + ' length: ' + curLength + ' of ' + objLength);
    }
}