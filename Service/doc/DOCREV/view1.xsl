<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:my="http://schemas.microsoft.com/office/infopath/2003/myXSD/2016-01-16T03:31:50" xmlns:xd="http://schemas.microsoft.com/office/infopath/2003" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:msxsl="urn:schemas-microsoft-com:xslt" xmlns:x="urn:schemas-microsoft-com:office:excel" xmlns:xdExtension="http://schemas.microsoft.com/office/infopath/2003/xslt/extension" xmlns:xdXDocument="http://schemas.microsoft.com/office/infopath/2003/xslt/xDocument" xmlns:xdSolution="http://schemas.microsoft.com/office/infopath/2003/xslt/solution" xmlns:xdFormatting="http://schemas.microsoft.com/office/infopath/2003/xslt/formatting" xmlns:xdImage="http://schemas.microsoft.com/office/infopath/2003/xslt/xImage" xmlns:xdUtil="http://schemas.microsoft.com/office/infopath/2003/xslt/Util" xmlns:xdMath="http://schemas.microsoft.com/office/infopath/2003/xslt/Math" xmlns:xdDate="http://schemas.microsoft.com/office/infopath/2003/xslt/Date" xmlns:sig="http://www.w3.org/2000/09/xmldsig#" xmlns:xdSignatureProperties="http://schemas.microsoft.com/office/infopath/2003/SignatureProperties" xmlns:ipApp="http://schemas.microsoft.com/office/infopath/2006/XPathExtension/ipApp" xmlns:xdEnvironment="http://schemas.microsoft.com/office/infopath/2006/xslt/environment" xmlns:xdUser="http://schemas.microsoft.com/office/infopath/2006/xslt/User" xmlns:xdServerInfo="http://schemas.microsoft.com/office/infopath/2009/xslt/ServerInfo">
	<xsl:output method="html" indent="no"/>
	<xsl:template match="my:DOCREV">
		<html>
			<head>
				<meta content="text/html" http-equiv="Content-Type"></meta>
				<style controlStyle="controlStyle">@media screen 			{ 			BODY{margin-left:21px;background-position:21px 0px;} 			} 		BODY{color:windowtext;background-color:window;layout-grid:none;} 		.xdListItem {display:inline-block;width:100%;vertical-align:text-top;} 		.xdListBox,.xdComboBox{margin:1px;} 		.xdInlinePicture{margin:1px; BEHAVIOR: url(#default#urn::xdPicture) } 		.xdLinkedPicture{margin:1px; BEHAVIOR: url(#default#urn::xdPicture) url(#default#urn::controls/Binder) } 		.xdHyperlinkBox{word-wrap:break-word; text-overflow:ellipsis;overflow-x:hidden; OVERFLOW-Y: hidden; WHITE-SPACE:nowrap; display:inline-block;margin:1px;padding:5px;border: 1pt solid #dcdcdc;color:windowtext;BEHAVIOR: url(#default#urn::controls/Binder) url(#default#DataBindingUI)} 		.xdSection{border:1pt solid transparent ;margin:0px 0px 0px 0px;padding:0px 0px 0px 0px;} 		.xdRepeatingSection{border:1pt solid transparent;margin:0px 0px 0px 0px;padding:0px 0px 0px 0px;} 		.xdMultiSelectList{margin:1px;display:inline-block; border:1pt solid #dcdcdc; padding:1px 1px 1px 5px; text-indent:0; color:windowtext; background-color:window; overflow:auto; behavior: url(#default#DataBindingUI) url(#default#urn::controls/Binder) url(#default#MultiSelectHelper) url(#default#ScrollableRegion);} 		.xdMultiSelectListItem{display:block;white-space:nowrap}		.xdMultiSelectFillIn{display:inline-block;white-space:nowrap;text-overflow:ellipsis;;padding:1px;margin:1px;border: 1pt solid #dcdcdc;overflow:hidden;text-align:left;}		.xdBehavior_Formatting {BEHAVIOR: url(#default#urn::controls/Binder) url(#default#Formatting);} 	 .xdBehavior_FormattingNoBUI{BEHAVIOR: url(#default#CalPopup) url(#default#urn::controls/Binder) url(#default#Formatting);} 	.xdExpressionBox{margin: 1px;padding:1px;word-wrap: break-word;text-overflow: ellipsis;overflow-x:hidden;}.xdBehavior_GhostedText,.xdBehavior_GhostedTextNoBUI{BEHAVIOR: url(#default#urn::controls/Binder) url(#default#TextField) url(#default#GhostedText);}	.xdBehavior_GTFormatting{BEHAVIOR: url(#default#urn::controls/Binder) url(#default#Formatting) url(#default#GhostedText);}	.xdBehavior_GTFormattingNoBUI{BEHAVIOR: url(#default#CalPopup) url(#default#urn::controls/Binder) url(#default#Formatting) url(#default#GhostedText);}	.xdBehavior_Boolean{BEHAVIOR: url(#default#urn::controls/Binder) url(#default#BooleanHelper);}	.xdBehavior_Select{BEHAVIOR: url(#default#urn::controls/Binder) url(#default#SelectHelper);}	.xdBehavior_ComboBox{BEHAVIOR: url(#default#ComboBox)} 	.xdBehavior_ComboBoxTextField{BEHAVIOR: url(#default#ComboBoxTextField);} 	.xdRepeatingTable{BORDER-TOP-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none; BORDER-BOTTOM-STYLE: none; BORDER-COLLAPSE: collapse; WORD-WRAP: break-word;}.xdScrollableRegion{BEHAVIOR: url(#default#ScrollableRegion);} 		.xdLayoutRegion{display:inline-block;} 		.xdMaster{BEHAVIOR: url(#default#MasterHelper);} 		.xdActiveX{margin:1px; BEHAVIOR: url(#default#ActiveX);} 		.xdFileAttachment{display:inline-block;margin:1px;BEHAVIOR:url(#default#urn::xdFileAttachment);} 		.xdSharePointFileAttachment{display:inline-block;margin:2px;BEHAVIOR:url(#default#xdSharePointFileAttachment);} 		.xdAttachItem{display:inline-block;width:100%%;height:25px;margin:1px;BEHAVIOR:url(#default#xdSharePointFileAttachItem);} 		.xdSignatureLine{display:inline-block;margin:1px;background-color:transparent;border:1pt solid transparent;BEHAVIOR:url(#default#SignatureLine);} 		.xdHyperlinkBoxClickable{behavior: url(#default#HyperlinkBox)} 		.xdHyperlinkBoxButtonClickable{border-width:1px;border-style:outset;behavior: url(#default#HyperlinkBoxButton)} 		.xdPictureButton{background-color: transparent; padding: 0px; behavior: url(#default#PictureButton);} 		.xdPageBreak{display: none;}BODY{margin-right:21px;} 		.xdTextBoxRTL{display:inline-block;white-space:nowrap;text-overflow:ellipsis;;padding:1px;margin:1px;border: 1pt solid #dcdcdc;color:windowtext;background-color:window;overflow:hidden;text-align:right;word-wrap:normal;} 		.xdRichTextBoxRTL{display:inline-block;;padding:1px;margin:1px;border: 1pt solid #dcdcdc;color:windowtext;background-color:window;overflow-x:hidden;word-wrap:break-word;text-overflow:ellipsis;text-align:right;font-weight:normal;font-style:normal;text-decoration:none;vertical-align:baseline;} 		.xdDTTextRTL{height:100%;width:100%;margin-left:22px;overflow:hidden;padding:0px;white-space:nowrap;} 		.xdDTButtonRTL{margin-right:-21px;height:17px;width:20px;behavior: url(#default#DTPicker);} 		.xdMultiSelectFillinRTL{display:inline-block;white-space:nowrap;text-overflow:ellipsis;;padding:1px;margin:1px;border: 1pt solid #dcdcdc;overflow:hidden;text-align:right;}.xdTextBox{display:inline-block;white-space:nowrap;text-overflow:ellipsis;;padding:1px;margin:1px;border: 1pt solid #dcdcdc;color:windowtext;background-color:window;overflow:hidden;text-align:left;word-wrap:normal;} 		.xdRichTextBox{display:inline-block;;padding:1px;margin:1px;border: 1pt solid #dcdcdc;color:windowtext;background-color:window;overflow-x:hidden;word-wrap:break-word;text-overflow:ellipsis;text-align:left;font-weight:normal;font-style:normal;text-decoration:none;vertical-align:baseline;} 		.xdDTPicker{;display:inline;margin:1px;margin-bottom: 2px;border: 1pt solid #dcdcdc;color:windowtext;background-color:window;overflow:hidden;text-indent:0; layout-grid: none} 		.xdDTText{height:100%;width:100%;margin-right:22px;overflow:hidden;padding:0px;white-space:nowrap;} 		.xdDTButton{margin-left:-21px;height:17px;width:20px;behavior: url(#default#DTPicker);} 		.xdRepeatingTable TD {VERTICAL-ALIGN: top;}</style>
				<style tableEditor="TableStyleRulesID">TABLE.xdLayout TD {
	BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none
}
TABLE.msoUcTable TD {
	BORDER-TOP: 1pt solid; BORDER-RIGHT: 1pt solid; BORDER-BOTTOM: 1pt solid; BORDER-LEFT: 1pt solid
}
TABLE {
	BEHAVIOR: url (#default#urn::tables/NDTable)
}
</style>
				<style languageStyle="languageStyle">BODY {
	FONT-SIZE: 10pt; FONT-FAMILY: Calibri
}
SELECT {
	FONT-SIZE: 10pt; FONT-FAMILY: Calibri
}
TABLE {
	FONT-SIZE: 10pt; FONT-FAMILY: Calibri; TEXT-TRANSFORM: none; FONT-WEIGHT: normal; COLOR: black; FONT-STYLE: normal
}
.optionalPlaceholder {
	FONT-SIZE: 9pt; TEXT-DECORATION: none; FONT-FAMILY: Calibri; FONT-WEIGHT: normal; COLOR: #333333; FONT-STYLE: normal; PADDING-LEFT: 20px; BEHAVIOR: url(#default#xOptional)
}
.langFont {
	FONT-SIZE: 10pt; FONT-FAMILY: Calibri; WIDTH: 150px
}
.defaultInDocUI {
	FONT-SIZE: 9pt; FONT-FAMILY: Calibri
}
.optionalPlaceholder {
	PADDING-RIGHT: 20px
}
</style>
				<style themeStyle="urn:office.microsoft.com:themeSummer">TABLE {
	BORDER-TOP: medium none; BORDER-RIGHT: medium none; BORDER-COLLAPSE: collapse; BORDER-BOTTOM: medium none; BORDER-LEFT: medium none
}
TD {
	BORDER-TOP-COLOR: black; BORDER-LEFT-COLOR: black; BORDER-BOTTOM-COLOR: black; BORDER-RIGHT-COLOR: black
}
TH {
	BORDER-TOP-COLOR: #000000; BORDER-LEFT-COLOR: #000000; COLOR: black; BORDER-BOTTOM-COLOR: #000000; BORDER-RIGHT-COLOR: #000000; BACKGROUND-COLOR: #f2f2f2
}
.xdTableHeader {
	COLOR: black; BACKGROUND-COLOR: #f2f2f2
}
.light1 {
	BACKGROUND-COLOR: #ffffff
}
.dark1 {
	BACKGROUND-COLOR: #000000
}
.light2 {
	BACKGROUND-COLOR: #f7f8f4
}
.dark2 {
	BACKGROUND-COLOR: #2b4b4d
}
.accent1 {
	BACKGROUND-COLOR: #6c9a7f
}
.accent2 {
	BACKGROUND-COLOR: #bb523d
}
.accent3 {
	BACKGROUND-COLOR: #c89d11
}
.accent4 {
	BACKGROUND-COLOR: #fccf10
}
.accent5 {
	BACKGROUND-COLOR: #568ea1
}
.accent6 {
	BACKGROUND-COLOR: #decf28
}
</style>
				<style tableStyle="Professional">TR.xdTitleRow {
	MIN-HEIGHT: 83px
}
TD.xdTitleCell {
	BORDER-TOP: #bfbfbf 1pt solid; BORDER-RIGHT: #bfbfbf 1pt solid; PADDING-BOTTOM: 14px; TEXT-ALIGN: center; PADDING-TOP: 32px; PADDING-LEFT: 22px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #ffffff; valign: bottom
}
TR.xdTitleRowWithHeading {
	MIN-HEIGHT: 69px
}
TD.xdTitleCellWithHeading {
	BORDER-TOP: #bfbfbf 1pt solid; BORDER-RIGHT: #bfbfbf 1pt solid; PADDING-BOTTOM: 0px; TEXT-ALIGN: center; PADDING-TOP: 32px; PADDING-LEFT: 22px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #ffffff; valign: bottom
}
TR.xdTitleRowWithSubHeading {
	MIN-HEIGHT: 75px
}
TD.xdTitleCellWithSubHeading {
	BORDER-TOP: #bfbfbf 1pt solid; BORDER-RIGHT: #bfbfbf 1pt solid; PADDING-BOTTOM: 6px; TEXT-ALIGN: center; PADDING-TOP: 32px; PADDING-LEFT: 22px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #ffffff; valign: bottom
}
TR.xdTitleRowWithOffsetBody {
	MIN-HEIGHT: 72px
}
TD.xdTitleCellWithOffsetBody {
	BORDER-TOP: #bfbfbf 1pt solid; BORDER-RIGHT: #bfbfbf 1pt solid; PADDING-BOTTOM: 2px; TEXT-ALIGN: left; PADDING-TOP: 32px; PADDING-LEFT: 22px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #ffffff; valign: bottom
}
TR.xdTitleHeadingRow {
	MIN-HEIGHT: 37px
}
TD.xdTitleHeadingCell {
	BORDER-RIGHT: #bfbfbf 1pt solid; PADDING-BOTTOM: 14px; TEXT-ALIGN: center; PADDING-TOP: 0px; PADDING-LEFT: 22px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #ffffff; valign: top
}
TR.xdTitleSubheadingRow {
	MIN-HEIGHT: 70px
}
TD.xdTitleSubheadingCell {
	BORDER-RIGHT: #bfbfbf 1pt solid; PADDING-BOTTOM: 16px; PADDING-TOP: 8px; PADDING-LEFT: 22px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #ffffff; valign: top
}
TD.xdVerticalFill {
	BORDER-TOP: #bfbfbf 1pt solid; BORDER-BOTTOM: #bfbfbf 1pt solid; BORDER-LEFT: #bfbfbf 1pt solid; BACKGROUND-COLOR: #354d3f
}
TD.xdTableContentCellWithVerticalOffset {
	BORDER-RIGHT: #bfbfbf 1pt solid; BORDER-BOTTOM: #bfbfbf 1pt solid; PADDING-BOTTOM: 2px; TEXT-ALIGN: left; PADDING-TOP: 32px; PADDING-LEFT: 95px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 0px; BACKGROUND-COLOR: #ffffff; valign: bottom
}
TR.xdTableContentRow {
	MIN-HEIGHT: 140px
}
TD.xdTableContentCell {
	BORDER-RIGHT: #bfbfbf 1pt solid; BORDER-BOTTOM: #bfbfbf 1pt solid; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 0px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 0px; BACKGROUND-COLOR: #ffffff; valign: top
}
TD.xdTableContentCellWithVerticalFill {
	BORDER-RIGHT: #bfbfbf 1pt solid; BORDER-BOTTOM: #bfbfbf 1pt solid; PADDING-BOTTOM: 0px; PADDING-TOP: 0px; PADDING-LEFT: 1px; BORDER-LEFT: #bfbfbf 1pt solid; PADDING-RIGHT: 1px; BACKGROUND-COLOR: #ffffff; valign: top
}
TD.xdTableStyleOneCol {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 22px; PADDING-RIGHT: 22px
}
TR.xdContentRowOneCol {
	MIN-HEIGHT: 45px; valign: center
}
TR.xdHeadingRow {
	MIN-HEIGHT: 27px
}
TD.xdHeadingCell {
	BORDER-TOP: #a6c2b2 1pt solid; BORDER-BOTTOM: #a6c2b2 1pt solid; PADDING-BOTTOM: 2px; TEXT-ALIGN: center; PADDING-TOP: 2px; PADDING-LEFT: 22px; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #e1eae5; valign: bottom
}
TR.xdSubheadingRow {
	MIN-HEIGHT: 28px
}
TD.xdSubheadingCell {
	BORDER-BOTTOM: #a6c2b2 1pt solid; PADDING-BOTTOM: 4px; TEXT-ALIGN: center; PADDING-TOP: 4px; PADDING-LEFT: 22px; PADDING-RIGHT: 22px; valign: bottom
}
TR.xdHeadingRowEmphasis {
	MIN-HEIGHT: 27px
}
TD.xdHeadingCellEmphasis {
	BORDER-TOP: #a6c2b2 1pt solid; BORDER-BOTTOM: #a6c2b2 1pt solid; PADDING-BOTTOM: 2px; TEXT-ALIGN: center; PADDING-TOP: 2px; PADDING-LEFT: 22px; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #e1eae5; valign: bottom
}
TR.xdSubheadingRowEmphasis {
	MIN-HEIGHT: 28px
}
TD.xdSubheadingCellEmphasis {
	BORDER-BOTTOM: #a6c2b2 1pt solid; PADDING-BOTTOM: 4px; TEXT-ALIGN: center; PADDING-TOP: 4px; PADDING-LEFT: 22px; PADDING-RIGHT: 22px; valign: bottom
}
TR.xdTableLabelControlStackedRow {
	MIN-HEIGHT: 45px
}
TD.xdTableLabelControlStackedCellLabel {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 22px; PADDING-RIGHT: 5px
}
TD.xdTableLabelControlStackedCellComponent {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 5px; PADDING-RIGHT: 22px
}
TR.xdTableRow {
	MIN-HEIGHT: 30px
}
TD.xdTableCellLabel {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 22px; PADDING-RIGHT: 5px
}
TD.xdTableCellComponent {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 5px; PADDING-RIGHT: 22px
}
TD.xdTableMiddleCell {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 5px; PADDING-RIGHT: 5px
}
TR.xdTableEmphasisRow {
	MIN-HEIGHT: 30px
}
TD.xdTableEmphasisCellLabel {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 22px; PADDING-RIGHT: 5px; BACKGROUND-COLOR: #c4d6cb
}
TD.xdTableEmphasisCellComponent {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 5px; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #c4d6cb
}
TD.xdTableMiddleCellEmphasis {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 5px; PADDING-RIGHT: 5px; BACKGROUND-COLOR: #c4d6cb
}
TR.xdTableOffsetRow {
	MIN-HEIGHT: 30px
}
TD.xdTableOffsetCellLabel {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 22px; PADDING-RIGHT: 5px; BACKGROUND-COLOR: #c4d6cb
}
TD.xdTableOffsetCellComponent {
	PADDING-BOTTOM: 4px; PADDING-TOP: 4px; PADDING-LEFT: 5px; PADDING-RIGHT: 22px; BACKGROUND-COLOR: #c4d6cb
}
P {
	FONT-SIZE: 11pt; COLOR: #354d3f; MARGIN-TOP: 0px
}
H1 {
	MARGIN-BOTTOM: 0px; FONT-SIZE: 24pt; FONT-WEIGHT: normal; COLOR: #354d3f; MARGIN-TOP: 0px
}
H2 {
	MARGIN-BOTTOM: 0px; FONT-SIZE: 16pt; FONT-WEIGHT: bold; COLOR: #354d3f; MARGIN-TOP: 0px
}
H3 {
	MARGIN-BOTTOM: 0px; FONT-SIZE: 12pt; TEXT-TRANSFORM: uppercase; FONT-WEIGHT: normal; COLOR: #354d3f; MARGIN-TOP: 0px
}
H4 {
	MARGIN-BOTTOM: 0px; FONT-SIZE: 10pt; FONT-WEIGHT: normal; COLOR: #262626; FONT-STYLE: italic; MARGIN-TOP: 0px
}
H5 {
	MARGIN-BOTTOM: 0px; FONT-SIZE: 10pt; FONT-WEIGHT: bold; COLOR: #354d3f; FONT-STYLE: italic; MARGIN-TOP: 0px
}
H6 {
	MARGIN-BOTTOM: 0px; FONT-SIZE: 10pt; FONT-WEIGHT: normal; COLOR: #262626; MARGIN-TOP: 0px
}
BODY {
	COLOR: black
}
</style>
			</head>
			<body>
				<div align="center">
					<table class="xdFormLayout" style="BORDER-TOP-STYLE: none; WORD-WRAP: break-word; WIDTH: 651px; BORDER-COLLAPSE: collapse; TABLE-LAYOUT: fixed; BORDER-BOTTOM-STYLE: none; BORDER-RIGHT-STYLE: none; BORDER-LEFT-STYLE: none">
						<colgroup>
							<col style="WIDTH: 651px"></col>
						</colgroup>
						<tbody>
							<tr class="xdTitleRow" style="MIN-HEIGHT: 106px">
								<td style="VERTICAL-ALIGN: middle" class="xdTitleCell">
									<div align="center"><span class="xdFileAttachment" hideFocus="1" style="HEIGHT: 30px; WIDTH: 161px" tabIndex="0" tabStop="true" xd:boundProp="xd:inline" xd:xctname="FileAttachment" xd:CtrlId="CTRL1" xd:binding="my:TargetDocTypeFiles">
											<xsl:if test="function-available('xdImage:getImageUrl')">
												<xsl:attribute name="src"><xsl:value-of select="xdImage:getImageUrl(my:TargetDocTypeFiles)"/></xsl:attribute>
											</xsl:if>
										</span><span class="xdFileAttachment" hideFocus="1" style="HEIGHT: 30px; WIDTH: 161px" tabIndex="0" tabStop="true" xd:boundProp="xd:inline" xd:xctname="FileAttachment" xd:CtrlId="CTRL5" xd:binding="my:TargetDocTypeDotNetAssembly">
											<xsl:if test="function-available('xdImage:getImageUrl')">
												<xsl:attribute name="src"><xsl:value-of select="xdImage:getImageUrl(my:TargetDocTypeDotNetAssembly)"/></xsl:attribute>
											</xsl:if>
										</span>
									</div>
									<div align="center"> </div>
									<h1 align="center"><span title="" class="xdExpressionBox xdDataBindingUI" tabIndex="-1" xd:xctname="ExpressionBox" xd:CtrlId="CTRL3" xd:binding="my:TargetDocTypeName" xd:disableEditing="yes">
											<xsl:value-of select="my:TargetDocTypeName"/>
										</span>
									</h1>
									<h1 align="center"><span title="" class="xdExpressionBox xdDataBindingUI" tabIndex="-1" xd:xctname="ExpressionBox" xd:CtrlId="CTRL4" xd:binding="my:TargetDocTypeVer" xd:disableEditing="yes" style="WHITE-SPACE: normal">
											<xsl:value-of select="my:TargetDocTypeVer"/>
										</span>
										<span class="xdlabel"></span><span title="" class="xdTextBox" hideFocus="1" tabIndex="0" xd:xctname="PlainText" xd:CtrlId="CTRL6" xd:binding="my:TargetDocMD5" style="WIDTH: 100%">
											<xsl:value-of select="my:TargetDocMD5"/>
										</span>
									</h1>
								</td>
							</tr>
							<tr class="xdTableContentRow">
								<td style="VERTICAL-ALIGN: middle" class="xdTableContentCell">
									<font size="2">
										<div align="center"> </div>
										<div align="center"> </div>
										<div align="center"> </div>
										<h2 style="FONT-WEIGHT: normal">
											<strong>Revision and version usage in standards</strong>
										</h2>
										<div style="FONT-WEIGHT: normal"> </div>
										<div>MIL-STD-100 only uses "version" (once) to refer to a part. Based on the context, we believe that the more appropriate term would be "variation":</div>
										<div> </div>
										<div>406.13 Change requiring new identification. When a repair part within an item is changed so that it is no longer interchangeable with its previous version, it shall be assigned a new PIN.</div>
										<div> </div>
										<div>The term "revision" is used extensively. As an example:</div>
										<div>The CAGE Code, drawing format size letter, and drawing revision letter are not considered part of the drawing number.</div>
										<div>MIL-HDBK-61A, interestingly, seems there was no need to actually define the term "revision", though its meaning is presumed by context:</div>
										<div> </div>
										<div>Configuration baseline (baseline). (2) An approved and released document, or a set of documents, each of a specific revision;…</div>
										<div> </div>
										<div>Notice of Revision (NOR). A document used to define revisions to configuration documentation which require revision after Engineering Change Proposal approval.</div>
										<div>However, a version is explicitly defined</div>
										<div> </div>
										<div>(1) One of several sequentially created configurations of a data product.</div>
										<div>(2) A supplementary identifier used to distinguish a changed body or set of computer-based data (software) from the previous configuration with the same primary identifier. Version identifiers are usually associated with data (such as files, databases and software) used by, or maintained in, computers.</div>
										<div> </div>
										<div>And, reinforcing that <font style="BACKGROUND-COLOR: #ffff00">documents have revisions</font> while software has versions (pg 4-34):</div>
										<div>ACTIVITY: Configuration Status Accounting: Identify data file(s) and document representations of revisions/versions of each document/software delivered, or made accessible electronically</div>
										<div> </div>
										<div>Finally, on page 5-19 (citing ASME Y14.55M):</div>
										<div>Revisions to Drawings</div>
										<div>- Drawing revision identification</div>
										<div>- Any change to a drawing, including a change to Rights-in-Data, must be recorded in the revisions block of the affected drawing.</div>
										<div>- Record revision status, identification of change authorization documents, or description of changes, and change approvals, and if multi-sheet, revision status of sheets</div>
										<div>MIL-STD-973 has some interesting contextual definitions:</div>
										<div>3.70 Notice of Revision (NOR) . A document used to define revisions to drawings, associated lists, or other referenced documents which require revision after Engineering Change Proposal approval.</div>
										<div>5.3.5.2.4 Consolidation of multiple changes into a single unrelated ECPS may be combined into a single revision to a document provided that:</div>
										<div> </div>
										<div>a. All changes apply to the same end item.</div>
										<div>b. All changes apply to the same revision/version…</div>
										<div> </div>
										<div>5.3.6.5 Software identifiers. For each CSCI, the contractor shall identify its corresponding Computer Software Components (CSCs) and Computer Software Units (CSUs). For each CSCI, CSC, and CSU the contractor shall issue/obtain a software identifier, which shall consist of a name or number, and a version identifier, and shall relate the software to its associated software design documentation; revision; and release date. The contractor shall embed the software and version identifiers within the source code, and provide a method for display of the software and version identifier data to the user upon command.</div>
										<div>EIA-649-1998: The EIA committee also didn't feel the need to explicitly define revisions, but uses the term throughout the standard:</div>
										<div> </div>
										<div>configuration identification: ...(3)...maintain[s] document revision relationships to product configurations.</div>
										<div>Methods of identifying documents...includes at least...organization, a unique identifier..., and a revision indicator (number, letter, or date).</div>
										<div> </div>
										<div>A version is defined using the MIL-HDBK-61A description.</div>
										<div> </div>
										<div>ISO 10007-1995, 7.4.1 has only one reference to "revision", and no reference to "version":</div>
										<div>...All change proposals should be documented and should typically include the following information prior to their submission to the CB:</div>
										<div>- configuration item(s) and related documents to be changed, name(s) and revision status; ...</div>
										<div>Since DOD-STD-2167A specifically addresses software development, it isn't surprising that "version" is used for a software variation:</div>
										<div> </div>
										<div>3.34 Version. An identified and documented body of software. Modifications to a version of software (resulting in a new version) require configuration management actions by either the contractor, the contracting agency, or both.</div>
										<div>But it is interesting that changes to DOD-STD-2167, as a document, are considered revisions:</div>
										<div> </div>
										<div>6.5 Changes from previous issue. Asterisks or vertical lines are not used in this revision to identify changes with respect to the previous issue due to the extensiveness of the changes.</div>
										<div> </div>
										<div>The IPC-257x standards for defining Product Data eXchange (PDX) files offers both revision (productRevision) and version (versionIdentifier) for parts, but only describes versions for electronic file attachments. While the IPC standards don't offer any distinction directly, revisions have somewhat superior standing; for instance, an item change history includes "oldRevision", "newRevision", "proposedRevision", "minimumShippableRevision" but there's no equivalent for versions. Likewise, (although we discourage it for other reasons) engineering changes are permitted to have revisions, but not versions. </div>
										<div> </div>
										<div align="right">
											<a href="http://www.product-lifecycle-management.com/plm-revision-version.htm" xd:disableEditing="yes">
												<em>http://www.product-lifecycle-management.com/plm-revision-version.htm</em>
											</a>
										</div>
									</font>
								</td>
							</tr>
						</tbody>
					</table>
				</div>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
