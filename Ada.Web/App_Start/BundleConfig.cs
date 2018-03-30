using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace Ada.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            // CSS style (bootstrap/inspinia)
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/animate.css",
                      "~/Content/style.css"));

            // Font Awesome icons
            bundles.Add(new StyleBundle("~/font-awesome/css").Include(
                      "~/fonts/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform()));

            // jQuery
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.1.1.min.js"));

            // jQueryUI CSS
            bundles.Add(new ScriptBundle("~/Scripts/plugins/jquery-ui/jqueryuiStyles").Include(
                        "~/Scripts/plugins/jquery-ui/jquery-ui.min.css"));

            // jQueryUI 
            bundles.Add(new StyleBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/plugins/jquery-ui/jquery-ui.min.js"));

            // Bootstrap
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js"));

            // Inspinia script
            bundles.Add(new ScriptBundle("~/bundles/inspinia").Include(
                      "~/Scripts/plugins/metisMenu/metisMenu.min.js",
                      "~/Scripts/plugins/pace/pace.min.js",
                      "~/Scripts/app/inspinia.js"));

            // Inspinia skin config script
            bundles.Add(new ScriptBundle("~/bundles/skinConfig").Include(
                      "~/Scripts/app/skin.config.min.js"));

            // SlimScroll
            bundles.Add(new ScriptBundle("~/plugins/slimScroll").Include(
                      "~/Scripts/plugins/slimscroll/jquery.slimscroll.min.js"));

            // Peity
            bundles.Add(new ScriptBundle("~/plugins/peity").Include(
                      "~/Scripts/plugins/peity/jquery.peity.min.js"));

            // Video responsible
            bundles.Add(new ScriptBundle("~/plugins/videoResponsible").Include(
                      "~/Scripts/plugins/video/responsible-video.js"));
            // Lightbox gallery css styles
            bundles.Add(new StyleBundle("~/plugins/blueimp").Include(
                      "~/Content/plugins/blueimp/css/blueimp-gallery.min.css",new CssRewriteUrlTransform()));
            // Lightbox gallery
            bundles.Add(new ScriptBundle("~/plugins/lightboxGallery").Include(
                      "~/Scripts/plugins/blueimp/jquery.blueimp-gallery.min.js"));

            // Sparkline
            bundles.Add(new ScriptBundle("~/plugins/sparkline").Include(
                      "~/Scripts/plugins/sparkline/jquery.sparkline.min.js"));

            // Morriss chart css styles
            bundles.Add(new StyleBundle("~/plugins/morrisStyles").Include(
                      "~/Content/plugins/morris/morris-0.4.3.min.css"));

            // Morriss chart
            bundles.Add(new ScriptBundle("~/plugins/morris").Include(
                      "~/Scripts/plugins/morris/raphael-2.1.0.min.js",
                      "~/Scripts/plugins/morris/morris.js"));

            // Flot chart
            bundles.Add(new ScriptBundle("~/plugins/flot").Include(
                      "~/Scripts/plugins/flot/jquery.flot.js",
                      "~/Scripts/plugins/flot/jquery.flot.tooltip.min.js",
                      "~/Scripts/plugins/flot/jquery.flot.resize.js",
                      "~/Scripts/plugins/flot/jquery.flot.pie.js",
                      "~/Scripts/plugins/flot/jquery.flot.time.js",
                      "~/Scripts/plugins/flot/jquery.flot.spline.js"));

            // Rickshaw chart
            bundles.Add(new ScriptBundle("~/plugins/rickshaw").Include(
                      "~/Scripts/plugins/rickshaw/vendor/d3.v3.js",
                      "~/Scripts/plugins/rickshaw/rickshaw.min.js"));

            // ChartJS chart
            bundles.Add(new ScriptBundle("~/plugins/chartJs").Include(
                      "~/Scripts/plugins/chartjs/Chart.min.js"));

            // iCheck css styles
            bundles.Add(new StyleBundle("~/Content/plugins/iCheck/iCheckStyles").Include(
                      "~/Content/plugins/iCheck/custom.css"));

            // iCheck
            bundles.Add(new ScriptBundle("~/plugins/iCheck").Include(
                      "~/Scripts/plugins/iCheck/icheck.min.js"));

            // dataTables css styles
            bundles.Add(new StyleBundle("~/Content/plugins/dataTables/dataTablesStyles").Include(
                      "~/Content/plugins/dataTables/datatables.min.css"));

            // dataTables 
            bundles.Add(new ScriptBundle("~/plugins/dataTables").Include(
                      "~/Scripts/plugins/dataTables/datatables.min.js"));

            // jeditable 
            bundles.Add(new ScriptBundle("~/plugins/jeditable").Include(
                      "~/Scripts/plugins/jeditable/jquery.jeditable.js"));

            // jqGrid styles
            bundles.Add(new StyleBundle("~/Content/plugins/jqGrid/jqGridStyles").Include(
                      "~/Content/plugins/jqGrid/ui.jqgrid.css"));

            // jqGrid 
            bundles.Add(new ScriptBundle("~/plugins/jqGrid").Include(
                      "~/Scripts/plugins/jqGrid/i18n/grid.locale-en.js",
                      "~/Scripts/plugins/jqGrid/jquery.jqGrid.min.js"));

            // codeEditor styles
            bundles.Add(new StyleBundle("~/plugins/codeEditorStyles").Include(
                      "~/Content/plugins/codemirror/codemirror.css",
                      "~/Content/plugins/codemirror/ambiance.css"));

            // codeEditor 
            bundles.Add(new ScriptBundle("~/plugins/codeEditor").Include(
                      "~/Scripts/plugins/codemirror/codemirror.js",
                      "~/Scripts/plugins/codemirror/mode/javascript/javascript.js"));

            // codeEditor 
            bundles.Add(new ScriptBundle("~/plugins/nestable").Include(
                      "~/Scripts/plugins/nestable/jquery.nestable.js"));

            // validate 
            bundles.Add(new ScriptBundle("~/plugins/validate").Include(
                      "~/Scripts/plugins/validate/jquery.validate.min.js",
                      "~/Scripts/plugins/validate/localization/messages_zh.min.js"));

            // fullCalendar styles
            bundles.Add(new StyleBundle("~/plugins/fullCalendarStyles").Include(
                      "~/Content/plugins/fullcalendar/fullcalendar.css"));

            // fullCalendar 
            bundles.Add(new ScriptBundle("~/plugins/fullCalendar").Include(
                      "~/Scripts/plugins/fullcalendar/moment.min.js",
                      "~/Scripts/plugins/fullcalendar/fullcalendar.min.js"));
            // monent
            bundles.Add(new ScriptBundle("~/plugins/moment").Include(
                "~/Scripts/plugins/fullcalendar/moment-with-locales.min.js"));
            // vectorMap 
            bundles.Add(new ScriptBundle("~/plugins/vectorMap").Include(
                      "~/Scripts/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                      "~/Scripts/plugins/jvectormap/jquery-jvectormap-world-mill-en.js"));

            // ionRange styles
            bundles.Add(new StyleBundle("~/Content/plugins/ionRangeSlider/ionRangeStyles").Include(
                      "~/Content/plugins/ionRangeSlider/ion.rangeSlider.css",
                      "~/Content/plugins/ionRangeSlider/ion.rangeSlider.skinFlat.css"));

            // ionRange 
            bundles.Add(new ScriptBundle("~/plugins/ionRange").Include(
                      "~/Scripts/plugins/ionRangeSlider/ion.rangeSlider.min.js"));

            // dataPicker styles
            bundles.Add(new StyleBundle("~/plugins/dataPickerStyles").Include(
                      "~/Content/plugins/datapicker/bootstrap-datepicker3.min.css"));

            // dataPicker 
            bundles.Add(new ScriptBundle("~/plugins/dataPicker").Include(
                      "~/Scripts/plugins/datapicker/bootstrap-datepicker.min.js",
                      "~/Scripts/plugins/datapicker/locales/bootstrap-datepicker.zh-CN.min.js"));

            // nouiSlider styles
            bundles.Add(new StyleBundle("~/plugins/nouiSliderStyles").Include(
                      "~/Content/plugins/nouslider/jquery.nouislider.css"));

            // nouiSlider 
            bundles.Add(new ScriptBundle("~/plugins/nouiSlider").Include(
                      "~/Scripts/plugins/nouslider/jquery.nouislider.min.js"));

            // jasnyBootstrap styles
            bundles.Add(new StyleBundle("~/plugins/jasnyBootstrapStyles").Include(
                      "~/Content/plugins/jasny/jasny-bootstrap.min.css"));

            // jasnyBootstrap 
            bundles.Add(new ScriptBundle("~/plugins/jasnyBootstrap").Include(
                      "~/Scripts/plugins/jasny/jasny-bootstrap.min.js"));

            // switchery styles
            bundles.Add(new StyleBundle("~/plugins/switcheryStyles").Include(
                      "~/Content/plugins/switchery/switchery.css"));

            // switchery 
            bundles.Add(new ScriptBundle("~/plugins/switchery").Include(
                      "~/Scripts/plugins/switchery/switchery.js"));

            // chosen styles
            bundles.Add(new StyleBundle("~/Content/plugins/chosen/chosenStyles").Include(
                      "~/Content/plugins/chosen/bootstrap-chosen.css"));

            // chosen 
            bundles.Add(new ScriptBundle("~/plugins/chosen").Include(
                      "~/Scripts/plugins/chosen/chosen.jquery.js"));

            // knob 
            bundles.Add(new ScriptBundle("~/plugins/knob").Include(
                      "~/Scripts/plugins/jsKnob/jquery.knob.js"));

            // wizardSteps styles
            bundles.Add(new StyleBundle("~/plugins/wizardStepsStyles").Include(
                      "~/Content/plugins/steps/jquery.steps.css"));

            // wizardSteps 
            bundles.Add(new ScriptBundle("~/plugins/wizardSteps").Include(
                      "~/Scripts/plugins/steps/jquery.steps.min.js"));

            // dropZone styles
            bundles.Add(new StyleBundle("~/Content/plugins/dropzone/dropZoneStyles").Include(
                      "~/Content/plugins/dropzone/basic.css",
                      "~/Content/plugins/dropzone/dropzone.css"));

            // dropZone 
            bundles.Add(new ScriptBundle("~/plugins/dropZone").Include(
                      "~/Scripts/plugins/dropzone/dropzone.js"));

            // summernote styles
            bundles.Add(new StyleBundle("~/plugins/summernoteStyles").Include(
                      "~/Content/plugins/summernote/summernote.css",
                      "~/Content/plugins/summernote/summernote-bs3.css"));

            // summernote 
            bundles.Add(new ScriptBundle("~/plugins/summernote").Include(
                      "~/Scripts/plugins/summernote/summernote.min.js"));

            // toastr notification 
            bundles.Add(new ScriptBundle("~/plugins/toastr").Include(
                      "~/Scripts/plugins/toastr/toastr.min.js"));

            // toastr notification styles
            bundles.Add(new StyleBundle("~/plugins/toastrStyles").Include(
                      "~/Content/plugins/toastr/toastr.min.css"));

            // color picker
            bundles.Add(new ScriptBundle("~/plugins/colorpicker").Include(
                      "~/Scripts/plugins/colorpicker/bootstrap-colorpicker.min.js"));

            // color picker styles
            bundles.Add(new StyleBundle("~/Content/plugins/colorpicker/colorpickerStyles").Include(
                      "~/Content/plugins/colorpicker/bootstrap-colorpicker.min.css"));

            // image cropper
            bundles.Add(new ScriptBundle("~/plugins/imagecropper").Include(
                      "~/Scripts/plugins/cropper/cropper.min.js"));

            // image cropper styles
            bundles.Add(new StyleBundle("~/plugins/imagecropperStyles").Include(
                      "~/Content/plugins/cropper/cropper.min.css"));

            // jsTree
            bundles.Add(new ScriptBundle("~/plugins/jsTree").Include(
                      "~/Scripts/plugins/jsTree/jstree.min.js"));
            // jsTree styles
            bundles.Add(new StyleBundle("~/plugins/jsTreeStyles").Include(
                      "~/Content/plugins/jsTree/css/style.min.css", new CssRewriteUrlTransform()));

            // Diff
            bundles.Add(new ScriptBundle("~/plugins/diff").Include(
                      "~/Scripts/plugins/diff_match_patch/javascript/diff_match_patch.js",
                      "~/Scripts/plugins/preetyTextDiff/jquery.pretty-text-diff.min.js"));

            // Idle timer
            bundles.Add(new ScriptBundle("~/plugins/idletimer").Include(
                      "~/Scripts/plugins/idle-timer/idle-timer.min.js"));

            // Tinycon
            bundles.Add(new ScriptBundle("~/plugins/tinycon").Include(
                      "~/Scripts/plugins/tinycon/tinycon.min.js"));

            // Chartist
            bundles.Add(new StyleBundle("~/plugins/chartistStyles").Include(
                      "~/Content/plugins/chartist/chartist.min.css"));

            // chartist styles
            bundles.Add(new ScriptBundle("~/plugins/chartist").Include(
                      "~/Scripts/plugins/chartist/chartist.min.js"));

            // Awesome bootstrap checkbox
            bundles.Add(new StyleBundle("~/plugins/awesomeCheckboxStyles").Include(
                      "~/Content/plugins/awesome-bootstrap-checkbox/awesome-bootstrap-checkbox.css"));

            // Clockpicker styles
            bundles.Add(new StyleBundle("~/plugins/clockpickerStyles").Include(
                      "~/Content/plugins/clockpicker/clockpicker.css"));

            // Clockpicker
            bundles.Add(new ScriptBundle("~/plugins/clockpicker").Include(
                      "~/Scripts/plugins/clockpicker/clockpicker.js"));

            // Date range picker Styless
            bundles.Add(new StyleBundle("~/plugins/dateRangeStyles").Include(
                      "~/Content/plugins/daterangepicker/daterangepicker.css"));

            // Date range picker
            bundles.Add(new ScriptBundle("~/plugins/dateRange").Include(
                      // Date range use moment.js same as full calendar plugin 
                      "~/Scripts/plugins/fullcalendar/moment.min.js",
                      "~/Scripts/plugins/daterangepicker/daterangepicker.js"));

            // Sweet alert Styless
            bundles.Add(new StyleBundle("~/plugins/sweetAlertStyles").Include(
                      "~/Content/plugins/sweetalert/sweetalert.css"));

            // Sweet alert
            bundles.Add(new ScriptBundle("~/plugins/sweetAlert").Include(
                      "~/Scripts/plugins/sweetalert/sweetalert-dev.js"));

            // Footable Styless
            bundles.Add(new StyleBundle("~/plugins/footableStyles").Include(
                      "~/Content/plugins/footable/footable.core.css", new CssRewriteUrlTransform()));
            bundles.Add(new StyleBundle("~/plugins/footableV3Styles").Include(
                "~/Content/plugins/footable/footable.bootstrap.min.css"));
            // Footable alert
            bundles.Add(new ScriptBundle("~/plugins/footable").Include(
                      "~/Scripts/plugins/footable/footable.all.min.js"));
            bundles.Add(new ScriptBundle("~/plugins/footableV3").Include(
                "~/Scripts/plugins/footable/footable.min.js"));
            // Select2 Styless
            bundles.Add(new StyleBundle("~/plugins/select2Styles").Include(
                      "~/Content/plugins/select2/select2.min.css"));

            // Select2
            bundles.Add(new ScriptBundle("~/plugins/select2").Include(
                      "~/Scripts/plugins/select2/select2.full.min.js", "~/Scripts/plugins/select2/i18n/zh-CN.js"));

            // Masonry
            bundles.Add(new ScriptBundle("~/plugins/masonry").Include(
                      "~/Scripts/plugins/masonary/masonry.pkgd.min.js"));

            // Slick carousel Styless
            bundles.Add(new StyleBundle("~/plugins/slickStyles").Include(
                      "~/Content/plugins/slick/slick.css", new CssRewriteUrlTransform()));

            // Slick carousel theme Styless
            bundles.Add(new StyleBundle("~/plugins/slickThemeStyles").Include(
                      "~/Content/plugins/slick/slick-theme.css", new CssRewriteUrlTransform()));

            // Slick carousel
            bundles.Add(new ScriptBundle("~/plugins/slick").Include(
                      "~/Scripts/plugins/slick/slick.min.js"));

            // Ladda buttons Styless
            bundles.Add(new StyleBundle("~/plugins/laddaStyles").Include(
                      "~/Content/plugins/ladda/ladda-themeless.min.css"));

            // Ladda buttons
            bundles.Add(new ScriptBundle("~/plugins/ladda").Include(
                      "~/Scripts/plugins/ladda/spin.min.js",
                      "~/Scripts/plugins/ladda/ladda.min.js",
                      "~/Scripts/plugins/ladda/ladda.jquery.min.js"));

            // Dotdotdot buttons
            bundles.Add(new ScriptBundle("~/plugins/truncate").Include(
                      "~/Scripts/plugins/dotdotdot/jquery.dotdotdot.min.js"));

            // Touch Spin Styless
            bundles.Add(new StyleBundle("~/plugins/touchSpinStyles").Include(
                      "~/Content/plugins/touchspin/jquery.bootstrap-touchspin.min.css"));

            // Touch Spin
            bundles.Add(new ScriptBundle("~/plugins/touchSpin").Include(
                      "~/Scripts/plugins/touchspin/jquery.bootstrap-touchspin.min.js"));

            // Tour Styless
            bundles.Add(new StyleBundle("~/plugins/tourStyles").Include(
                      "~/Content/plugins/bootstrapTour/bootstrap-tour.min.css"));

            // Tour Spin
            bundles.Add(new ScriptBundle("~/plugins/tour").Include(
                      "~/Scripts/plugins/bootstrapTour/bootstrap-tour.min.js"));

            // i18next Spin
            bundles.Add(new ScriptBundle("~/plugins/i18next").Include(
                      "~/Scripts/plugins/i18next/i18next.min.js"));

            // Clipboard Spin
            bundles.Add(new ScriptBundle("~/plugins/clipboard").Include(
                      "~/Scripts/plugins/clipboard/clipboard.min.js"));

            // c3 Styless
            bundles.Add(new StyleBundle("~/plugins/c3Styles").Include(
                      "~/Content/plugins/c3/c3.min.css"));

            // c3 Charts
            bundles.Add(new ScriptBundle("~/plugins/c3").Include(
                      "~/Scripts/plugins/c3/c3.min.js"));

            // D3
            bundles.Add(new ScriptBundle("~/plugins/d3").Include(
                      "~/Scripts/plugins/d3/d3.min.js"));

            // Markdown Styless
            bundles.Add(new StyleBundle("~/plugins/markdownStyles").Include(
                      "~/Content/plugins/bootstrap-markdown/bootstrap-markdown.min.css"));

            // Markdown 
            bundles.Add(new ScriptBundle("~/plugins/markdown").Include(
                      "~/Scripts/plugins/bootstrap-markdown/bootstrap-markdown.js",
                      "~/Scripts/plugins/bootstrap-markdown/markdown.js"));

            // Datamaps
            bundles.Add(new ScriptBundle("~/plugins/datamaps").Include(
                      "~/Scripts/plugins/topojson/topojson.js",
                      "~/Scripts/plugins/datamaps/datamaps.all.min.js"));

            // Taginputs Styless
            bundles.Add(new StyleBundle("~/plugins/tagInputsStyles").Include(
                      "~/Content/plugins/bootstrap-tagsinput/bootstrap-tagsinput.css"));

            // Taginputs
            bundles.Add(new ScriptBundle("~/plugins/tagInputs").Include(
                      "~/Scripts/plugins/bootstrap-tagsinput/bootstrap-tagsinput.js"));

            // Duallist Styless
            bundles.Add(new StyleBundle("~/plugins/duallistStyles").Include(
                      "~/Content/plugins/bootstrap-tagsinput/bootstrap-tagsinput.css"));

            // Duallist
            bundles.Add(new ScriptBundle("~/plugins/duallist").Include(
                      "~/Scripts/plugins/dualListbox/jquery.bootstrap-duallistbox.js"));

            // SocialButtons styles
            bundles.Add(new StyleBundle("~/plugins/socialButtonsStyles").Include(
                      "~/Content/plugins/bootstrapSocial/bootstrap-social.css"));

            // Typehead
            bundles.Add(new ScriptBundle("~/plugins/typehead").Include(
                      "~/Scripts/plugins/typehead/bootstrap3-typeahead.min.js"));

            // Pdfjs
            bundles.Add(new ScriptBundle("~/plugins/pdfjs").Include(
                      "~/Scripts/plugins/pdfjs/pdf.js"));

            // Touch Punch 
            bundles.Add(new StyleBundle("~/plugins/touchPunch").Include(
                        "~/Scripts/plugins/touchpunch/jquery.ui.touch-punch.min.js"));

            // WOW 
            bundles.Add(new StyleBundle("~/plugins/wow").Include(
                        "~/Scripts/plugins/wow/wow.min.js"));

            // Text spinners styles
            bundles.Add(new StyleBundle("~/plugins/textSpinnersStyles").Include(
                      "~/Content/plugins/textSpinners/spinners.css"));

            // Password meter 
            bundles.Add(new StyleBundle("~/plugins/passwordMeter").Include(
                        "~/Scripts/plugins/pwstrength/pwstrength-bootstrap.min.js",
                        "~/Scripts/plugins/pwstrength/zxcvbn.js"));

            // jQuery form auto fill
            bundles.Add(new ScriptBundle("~/plugins/formFill").Include(
                "~/Scripts/plugins/jquery-form-fill/jquery.formautofill.min.js"));

            // base Script
            bundles.Add(new ScriptBundle("~/bundles/base").Include(
                "~/Scripts/base.js"));

            //Bootstrap Table
            bundles.Add(new StyleBundle("~/plugins/bootstrapTableStyle").Include(
                "~/Content/plugins/bootstrap-table/bootstrap-table.min.css"));
            bundles.Add(new ScriptBundle("~/plugins/bootstrapTableScript").Include(
                "~/Scripts/plugins/bootstrap-table/bootstrap-table.min.js",
                "~/Scripts/plugins/bootstrap-table/extensions/mobile/bootstrap-table-mobile.min.js",
                "~/Scripts/plugins/bootstrap-table/locale/bootstrap-table-zh-CN.min.js"));
            //Bootstrap Table Edit
            bundles.Add(new StyleBundle("~/plugins/bootstrapEditTableStyle").Include(
                "~/Content/plugins/bootstrap-table/extensions/editable/css/bootstrap-editable.css",new CssRewriteUrlTransform()));
            bundles.Add(new ScriptBundle("~/plugins/bootstrapEditTableScript").Include(
                "~/Scripts/plugins/bootstrap-table/extensions/editable/bootstrap-table-editable.min.js",
                "~/Scripts/plugins/bootstrap-table/extensions/editable/bootstrap-editable.min.js"));
            //Bootstrap Table Export
            bundles.Add(new ScriptBundle("~/plugins/bootstrapTableExport").Include(
                "~/Scripts/plugins/bootstrap-table/extensions/export/bootstrap-table-export.min.js",
                "~/Scripts/plugins/bootstrap-table/extensions/export/tableExport.js"));
            //Bootstrap DateTime Picker
            bundles.Add(new StyleBundle("~/plugins/dateTimePickerStyle").Include(
                "~/Content/plugins/datetimepicker/bootstrap-datetimepicker.min.css"));
            bundles.Add(new ScriptBundle("~/plugins/dateTimePickerScript").Include(
                "~/Scripts/plugins/datetimepicker/bootstrap-datetimepicker.min.js",
                "~/Scripts/plugins/datetimepicker/locales/bootstrap-datetimepicker.zh-CN.js"));
            // lodash
            bundles.Add(new ScriptBundle("~/plugins/lodash").Include(
                "~/Scripts/plugins/lodash/lodash.min.js"));
            // html2canvas
            bundles.Add(new ScriptBundle("~/plugins/html2canvas").Include(
                "~/Scripts/plugins/html2canvas/html2canvas.js"));
            // 百度Echarts
            bundles.Add(new ScriptBundle("~/plugins/echarts").Include(
                "~/Scripts/plugins/echarts/echarts.min.js"));


            //===========================前端=============================
            //bootstrap 4.0
            bundles.Add(new StyleBundle("~/WebGlobal/bootstrapStyle").Include(
                "~/Assets/vendor/bootstrap/bootstrap.css"));
            bundles.Add(new ScriptBundle("~/WebGlobal/Jquery").Include(
                "~/Assets/vendor/jquery/jquery.min.js"));
            bundles.Add(new ScriptBundle("~/WebGlobal/bootstrapScript").Include(
                "~/Assets/vendor/jquery-migrate/jquery-migrate.min.js", 
                "~/Assets/vendor/popper.min.js",
                "~/Assets/vendor/bootstrap/bootstrap.js"));
            //Plugins
            bundles.Add(new StyleBundle("~/WebPlugins/fontawesomeStyle").Include(
                "~/Assets/vendor/icon-awesome/css/font-awesome.min.css"));

            bundles.Add(new StyleBundle("~/WebPlugins/iconlineproStyle").Include(
                "~/Assets/vendor/icon-line-pro/style.css"));

            bundles.Add(new StyleBundle("~/WebPlugins/iconhsStyle").Include(
                "~/Assets/vendor/icon-hs/style.css"));

            bundles.Add(new StyleBundle("~/WebPlugins/iconlinesimpleStyle").Include(
                "~/Assets/vendor/icon-line/css/simple-line-icons.css"));

            bundles.Add(new StyleBundle("~/WebPlugins/animateStyle").Include(
                "~/Assets/vendor/animate.css"));

            bundles.Add(new StyleBundle("~/WebPlugins/hsmegamenuStyle").Include(
                "~/Assets/vendor/hs-megamenu/src/hs.megamenu.css"));
            bundles.Add(new ScriptBundle("~/WebPlugins/hsmegamenuScript").Include(
                "~/Assets/vendor/hs-megamenu/src/hs.megamenu.js"));


            bundles.Add(new StyleBundle("~/WebPlugins/hamburgersStyle").Include(
                "~/Assets/vendor/hamburgers/hamburgers.min.css"));

            bundles.Add(new StyleBundle("~/WebPlugins/chosenStyle").Include(
                "~/Assets/vendor/chosen/chosen.css"));
            bundles.Add(new ScriptBundle("~/WebPlugins/chosenScript").Include(
                "~/Assets/vendor/chosen/chosen.jquery.js"));

            bundles.Add(new StyleBundle("~/WebPlugins/slickStyle").Include(
                "~/Assets/vendor/slick-carousel/slick/slick.css"));
            bundles.Add(new ScriptBundle("~/WebPlugins/slickScript").Include(
                "~/Assets/vendor/slick-carousel/slick/slick.js"));

            bundles.Add(new StyleBundle("~/WebPlugins/fancyboxStyle").Include(
                "~/Assets/vendor/fancybox/jquery.fancybox.min.css"));
            bundles.Add(new ScriptBundle("~/WebPlugins/fancyboxScript").Include(
                "~/Assets/vendor/fancybox/jquery.fancybox.min.js"));

            //Theme multipage-real-estate
            bundles.Add(new StyleBundle("~/WebTheme/realestateStyle").Include(
                "~/Assets/theme/real-estate/css/styles.multipage-real-estate.css"));

            //Jquery UI
            bundles.Add(new ScriptBundle("~/WebJquaryUI/widgetScript").Include(
                "~/Assets/vendor/jquery-ui/ui/widget.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/widgetmenuScript").Include(
                "~/Assets/vendor/jquery-ui/ui/widgets/menu.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/widgetmouseScript").Include(
                "~/Assets/vendor/jquery-ui/ui/widgets/mouse.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/widgetsliderScript").Include(
                "~/Assets/vendor/jquery-ui/ui/widgets/slider.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/versionScript").Include(
                "~/Assets/vendor/jquery-ui/ui/version.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/keycodeScript").Include(
                "~/Assets/vendor/jquery-ui/ui/keycode.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/positionScript").Include(
                "~/Assets/vendor/jquery-ui/ui/position.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/uniqueidScript").Include(
                "~/Assets/vendor/jquery-ui/ui/unique-id.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/safeactiveelementScript").Include(
                "~/Assets/vendor/jquery-ui/ui/safe-active-element.js"));
            bundles.Add(new ScriptBundle("~/WebJquaryUI/widgetdatepickerScript").Include(
                "~/Assets/vendor/jquery-ui/ui/widgets/datepicker.js"));

            //Unify JS
            bundles.Add(new ScriptBundle("~/WebUnify/core").Include(
                "~/Assets/js/hs.core.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/header").Include(
                "~/Assets/js/components/hs.header.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/hamburgers").Include(
                "~/Assets/js/helpers/hs.hamburgers.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/dropdown").Include(
                "~/Assets/js/components/hs.dropdown.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/slider").Include(
                "~/Assets/js/components/hs.slider.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/select").Include(
                "~/Assets/js/components/hs.select.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/carousel").Include(
                "~/Assets/js/components/hs.carousel.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/popup").Include(
                "~/Assets/js/components/hs.popup.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/datepicker").Include(
                "~/Assets/js/components/hs.datepicker.js"));
            bundles.Add(new ScriptBundle("~/WebUnify/goto").Include(
                "~/Assets/js/components/hs.go-to.js"));

            //modals
            bundles.Add(new StyleBundle("~/WebUnify/modalStyle").Include(
                "~/Assets/vendor/animate.css", "~/Assets/vendor/custombox/custombox.min.css"));
            bundles.Add(new ScriptBundle("~/WebUnify/modalScript").Include(
                "~/Assets/vendor/custombox/custombox.min.js", "~/Assets/js/components/hs.modal-window.js"));
        }
    }
}