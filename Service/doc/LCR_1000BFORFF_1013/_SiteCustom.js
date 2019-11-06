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
//  Main Code that Parses the Form Array into JsonString
//-------------------------------------------------------------------------------------------------------------------->
function getJsonStringFromSerialArray(objFormFields) {
    var sptName = '';
    var sptIndex = '';
    var sptObject = '';
    var objResult = '{';
    var objCount = objFormFields.length - 1;
    var fundCheckboxes = '';
    $.each(objFormFields, function (i) {
        //alert(this.name);
        //alert(this.value);
        var obj = this.name || '';
        if ((obj.indexOf('cb') < 0) && (obj.indexOf('ddl') < 0)) {
            //FIRST
            //alert("Inside cb condition");
            if (obj.indexOf('[') > -1) {
                var objName = obj.split('[');
                var objObject = obj.split('.');
                var objIndex = objName[1].split(']');
                if (sptName != objName[0]) {
                    if (sptName.length > 0) { objResult += '}]'; }
                    sptName = objName[0];
                    sptIndex = objIndex[0];
                    sptObject = objObject[1];
                    //alert('Part1'+(i > 0 ? ',' : '') + '"' + sptName + '":[{"' + sptObject + '":"' + this.value + '"');//"Opt":[{"Item1":""
                    objResult += (i > 0 ? ',' : '') + '"' + sptName + '":[{"' + sptObject + '":"' + this.value + '"';
                }
                else {
                    if (sptIndex == objIndex[0]) {
                        sptObject = objObject[1];
                        //alert(obj);
                        //alert(this.value);
                        //alert('Part2' + (i > 0 ? ',' : '') + '"' + sptObject + '":"' + this.value + '"'); //,"Item2":""
                        objResult += (i > 0 ? ',' : '') + '"' + sptObject + '":"' + this.value + '"';
                    }
                    else {
                        sptIndex = objIndex[0];
                        sptObject = objObject[1];
                        ////alert('Part3' + '}' + (i > 0 ? ',' : '') + '{"' + sptObject + '":"' + this.value + '"');
                        objResult += '}' + (i > 0 ? ',' : '') + '{"' + sptObject + '":"' + this.value + '"';
                    }
                }
                if (i == objCount) { objResult += '}]'; }
            }
            else {
                if (obj != sptName && sptName.length > 0) {
                    sptName = '';
                    sptIndex = '';
                    sptObject = '';
                    objResult += '}]';
                }
                ////alert('Part4' + (i > 0 ? ',' : '') + '"' + obj + '":"' + this.value + '"'); //DocTitle,DocTypeName etc.. below
                objResult += (i > 0 ? ',' : '') + '"' + obj + '":"' + this.value + '"';
            }
            if (i == objCount) { objResult += '}'; }
            //FIRST
        }
        //else 
        //{
        //THIS IS THE PLACE WHERE OUR CHECK BOXES AND DDLS LAND IN 
        //            var objName = obj.split('[');
        //            var objObject = obj.split('.');
        //            var objIndex = objName[1].split(']');

        //            sptName = objName[0];
        //            sptIndex = objIndex[0];
        //            sptObject = objObject[1];
        //            ////////alert(obj);
        //            ////////alert(this.value);
        //            if (i == objCount) { objResult += '}'; }
        //}
    });
    return objResult;
}

//-------------------------------------------------------------------------------------------------------------------->
//  Json Files
//-------------------------------------------------------------------------------------------------------------------->
function loadJsonFile(jsonFile) {
    //So far based on what I found this method may have to be each form sepcfic 
    //as of now this serves well to LCR_1023AFORPD_0715.htm

    if (jsonFile == null) { jsonFile = getJsonFileName(); }
    //$('img').each(function () { var src = $(this).attr('src'); if (src === undefined) { $(this).attr('src', 'Resources/Images/Spacer.gif'); } });
    $.getJSON(jsonFile, function (data) {
        var items = [];
        $.each(data, function (key, val) {
            if (val == null) { val = ''; }
            if (val.toString().indexOf('[object Object]') > -1) {
                for (var i = 0; i < val.length; i++) {
                    for (obj in val[i]) {
                        if (obj != "SignatureImage") {
                            if (key == 'Opt') {
                                if (obj == 'Item2') {
                                    //Its a checkbox now see whats the value(val[i][obj]) and mark that check box by using('#cb' + key + '_' + i + '__' + obj+'_1')
                                    //_1,_2,_3 alongside attribute as checked
                                    if (val[i][obj] == 'Yes') {//cbOpt_45__Item2_1
                                        $('#cb' + key + '_' + i + '__' + obj + '_1').attr('checked', true);
                                        //Opt[0].Item2
                                        //alert('#' + key + '_' + i + '__' + obj);
                                        //alert($('#' + key + '_' + i + '__' + obj).length);
                                        $('#' + key + '_' + i + '__' + obj).val("Yes");
                                        //Opt_14__Item2
                                    }
                                    else if (val[i][obj] == 'No') {//cbOpt_45__Item2_2
                                        $('#cb' + key + '_' + i + '__' + obj + '_2').attr('checked', true);
                                        //alert('#' + key + '_' + i + '__' + obj);
                                        //alert($('#' + key + '_' + i + '__' + obj).length);
                                        $('#' + key + '_' + i + '__' + obj).val("No");
                                        //////alert($('#' + key + '[' + i + '].' + obj).val());

                                    }
                                    else if (val[i][obj] == 'N/A') {//cbOpt_7__Item2_0
                                        $('#cb' + key + '_' + i + '__' + obj + '_0').attr('checked', true);
                                        //alert('#' + key + '_' + i + '__' + obj);
                                        //alert($('#' + key + '_' + i + '__' + obj).length);
                                        $('#' + key + '_' + i + '__' + obj).val("N/A");

                                        //$('#' + key + '[' + i + '].' + obj).val("N/A");
                                    }
                                    else if (val[i][obj] != '') {
                                        $('#' + key + '_' + i + '__' + obj).val(val[i][obj]);
                                        //alert(val[i][obj]);
                                        //alert(key);
                                        //alert(obj);
                                        //alert(i);
                                        //$('#' + key + '_' + i + '__' + obj).val("N/A");
                                    }
                                }
                                else {
                                    if (obj == 'Item1' && (val[i][obj].indexOf("Toilets") >= 0 || val[i][obj].indexOf("Showers") >= 0 || val[i][obj].indexOf("Sinks") >= 0)) {
                                        /*{
                                        "Item1": "Toilets: 1, Showers: 2, Sinks: 3",
                                        "Item2": "Yes",
                                        "Item3": "",
                                        "Item4": ""
                                        }*/
                                        var strTSSval = val[i][obj];
                                        var res = strTSSval.split(",");
                                        var counti; //var countj;
                                        for (counti = 0; counti < res.length; ++counti) {
                                            var res1 = res[counti].split(":");
                                            if ($.trim(res1[0]) == "Toilets") {
                                                //res1[1] has the number set it to selected in ddlToilets
                                                $("#ddlToilets").val($.trim(res1[1]));
                                                //Set the value of Hidden Variable
                                                //$('#' + key + '[' + i + '].' + obj).val("Yes");
                                                //ddlOpt_42__Item1
                                            }
                                            if ($.trim(res1[0]) == "Showers") {
                                                //res1[1] has the number set it to selected in ddlShowers
                                                $("#ddlShowers").val($.trim(res1[1]));
                                            }
                                            if ($.trim(res1[0]) == "Sinks") {
                                                //res1[1] has the number set it to selected in ddlSinks
                                                $("#ddlSinks").val($.trim(res1[1]));
                                            }
                                        }
                                    }
                                    else {
                                        $('#' + key + '_' + i + '__' + obj).val(val[i][obj]);
                                    }
                                }
                            }
                            else {
                                $('#' + key + '_' + i + '__' + obj).val(val[i][obj]);
                            }
                        }
                        else {
                            $('#' + 'img' + key + '_' + i + '__' + obj).attr("src", val[i][obj]);
                        }
                    }
                }
            }
            else { $('#' + key.replace('[', '_').replace(']', '_').replace('.', '_')).val(val); }

            // Populate Additional Objects 
            //-------------------------------------------------------------------------------------------------------------------->
            if ($('#img' + key).length > 0) { $('#img' + key).attr('src', val); }
            if ($('input:checkbox[name=cb' + key + ']').length > 0) {
                $('input:checkbox[name=cb' + key + ']').each(function () { if ($(this).val() == val) { $(this).attr('checked', true); } });
            }
        });
    });
}


function getJsonStringFromSerialArrayLCR_1005AFORFF_0415(objFormFields) {
    var sptName = '';
    var sptIndex = '';
    var sptObject = '';
    var objResult = '{';
    var objCount = objFormFields.length - 1;
    var fundCheckboxes = '';
    $.each(objFormFields, function (i) {
        var obj = this.name || '';
        if (i > 0) {
            objResult += '"' + sptName + '":[{"' + sptObject + '":"' + this.value + '"';
        }
        else {
        }
        if (i == objCount) { objResult += '}'; }
    });
    return objResult;
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

/*//-------------------------------------------------------------------------------------------------------------------->
Input Signatures - https://github.com/szimek/signature_pad
//-------------------------------------------------------------------------------------------------------------------->*/
//function openSignaturePad(obj) {
function openSignaturePad(obj, objType) {

    var xSize = 2;
    var objDialogId = "objSinaturePad";
    if (objType == 'Initials') { xSize = 3; objDialogId = "objInitialsPad"; }

    var objControlId = $(obj).attr("id").toString().substring(3);
    var w = $(obj).width() * xSize;
    var h = $(obj).height() * xSize;

    $('.objSignatureCanvas').attr('style', 'width: ' + w + 'px; height: ' + h + 'px; border: 1px solid Red;')
    $('#' + objDialogId).dialog({ resizable: false, height: h + 125, width: w + 40, modal: true });

    var wrapper = document.getElementById(objDialogId),
            clearButton = wrapper.querySelector('[data-action=clear]'),
            saveButton = wrapper.querySelector('[data-action=save]'),
            canvas = wrapper.querySelector('canvas'), signaturePad;

    canvas.width = w;
    canvas.height = h;

    signaturePad = new SignaturePad(canvas);
    signaturePad.clear();
    signaturePad.penColor = 'Navy';

    var clearSignature = function (event) { signaturePad.clear(); };
    var applySignature = function (event) {

        var srcSignature = '';
        if (!signaturePad.isEmpty()) { srcSignature = signaturePad.toDataURL(); }

        $('#' + objControlId).val(srcSignature);
        $('#img' + objControlId).attr('src', srcSignature);

        $(obj).removeClass('reqField');

        clearButton.removeEventListener('click', clearSignature);
        saveButton.removeEventListener('click', applySignature);

        $('#' + objDialogId).dialog('close');
    };

    clearButton.addEventListener('click', clearSignature);
    saveButton.addEventListener('click', applySignature);
}

//-------------------------------------------------------------------------------------------------------------------->
//  jQueryUI Dialog Popup
//-------------------------------------------------------------------------------------------------------------------->
//function openDialog(Name, Title, Width, Height, Position) {

//    var div = $(Name);
//    if (Title == null) { div.dialog({ modal: true, resizable: false, width: Width, height: Height, position: Position }); }
//    else { div.dialog({ modal: true, resizable: false, title: Title, width: Width, height: Height, position: Position }); }

//    div.dialog('open');

//    return div;
//}

//function showPopup(objPopup, objResponse, objTitle, objWidth, objHeigth) {
//    $(objPopup).load(objResponse);
//    openDialog(objPopup, objTitle, objWidth, objHeigth, 'middle');
//}
