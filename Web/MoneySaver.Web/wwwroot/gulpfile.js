'use strict'

var gulp = require('gulp');
var browserSync = require('browser-sync').create();
var sass = require('gulp-sass');
var rename = require('gulp-rename');
var del = require('del');
var replace = require('gulp-replace');
var injectPartials = require('gulp-inject-partials');
var inject = require('gulp-inject');
var sourcemaps = require('gulp-sourcemaps');
var concat = require('gulp-concat');
var merge = require('merge-stream');

gulp.paths = {
    dist: 'dist',
};

var paths = gulp.paths;


gulp.task('sass', function () {
    return gulp.src('./scss/**/style.scss')
        .pipe(sourcemaps.init())
        .pipe(sass({outputStyle: 'expanded'}).on('error', sass.logError))
        .pipe(sourcemaps.write('./maps'))
        .pipe(gulp.dest('./css'))
        .pipe(browserSync.stream());
});

// Static Server + watching scss/html files
gulp.task('serve', gulp.series('sass', function() {

    browserSync.init({
        port: 3000,
        server: "./",
        ghostMode: false,
        notify: false
    });

    gulp.watch('scss/**/*.scss', gulp.series('sass'));
    gulp.watch('**/*.html').on('change', browserSync.reload);
    gulp.watch('js/**/*.js').on('change', browserSync.reload);

}));



// Static Server without watching scss files
gulp.task('serve:lite', function() {

    browserSync.init({
        server: "./",
        ghostMode: false,
        notify: false
    });

    gulp.watch('**/*.css').on('change', browserSync.reload);
    gulp.watch('**/*.html').on('change', browserSync.reload);
    gulp.watch('js/**/*.js').on('change', browserSync.reload);

});



gulp.task('sass:watch', function () {
    gulp.watch('./scss/**/*.scss');
});


/* inject partials like sidebar and navbar */
gulp.task('injectPartial', function () {
  return gulp.src("./**/*.html", { base: "./" })
    .pipe(injectPartials())
    .pipe(gulp.dest("."));
});



/* inject Js and CCS assets into HTML */
gulp.task('injectCommonAssets', function () {
  return gulp.src('./**/*.html')
    .pipe(inject(gulp.src([ 
        './vendors/iconfonts/mdi/font/css/materialdesignicons.min.css',
        './vendors/css/vendor.bundle.base.css', 
        './vendors/css/vendor.bundle.addons.css',
        './vendors/js/vendor.bundle.base.js',
        './vendors/js/vendor.bundle.addons.js'
    ], {read: false}), {name: 'plugins', relative: true}))
    .pipe(inject(gulp.src([
        './css/*.css', 
        './js/off-canvas.js', 
        './js/hoverable-collapse.js', 
        './js/template.js', 
        './js/settings.js', 
        './js/todolist.js'
    ], {read: false}), {relative: true}))
    .pipe(gulp.dest('.'));
});

/* inject Js and CCS assets into HTML */
gulp.task('injectLayoutStyles', function () {
    var verticalLightStream = gulp.src(['./**/vertical-default-light/**/*.html',
            './**/vertical-boxed/**/*.html',
            './**/vertical-compact/**/*.html',
            './**/vertical-dark-sidebar/**/*.html',
            './**/vertical-fixed/**/*.html',
            './**/vertical-hidden-toggle/**/*.html',
            './**/vertical-icon-menu/**/*.html',
            './**/vertical-toggle-overlay/**/*.html',
            './index.html'])
        .pipe(inject(gulp.src([
            './css/vertical-layout-light/style.css', 
        ], {read: false}), {relative: true}))
        .pipe(gulp.dest('.'));
    var horizontalStream = gulp.src('./**/horizontal-default/**/*.html')
        .pipe(inject(gulp.src([
            './css/horizontal-layout/style.css', 
        ], {read: false}), {relative: true}))
        .pipe(gulp.dest('.'));
    var verticalDarkStream = gulp.src('./**/vertical-default-dark/**/*.html')
        .pipe(inject(gulp.src([
            './css/vertical-layout-dark/style.css', 
        ], {read: false}), {relative: true}))
        .pipe(gulp.dest('.'));
    return merge(verticalLightStream, horizontalStream, verticalDarkStream);
});

/*replace image path and linking after injection*/
gulp.task('replacePath', function(){
    var replacePath1 = gulp.src(['./demo/*/pages/*/*.html'], { base: "./" })
        .pipe(replace('="images/', '="../../../../images/'))
        .pipe(replace('href="pages/', 'href="../../pages/'))
        .pipe(replace('href="index.html"', 'href="../../index.html"'))
        .pipe(gulp.dest('.'));
    var replacePath2 = gulp.src(['./demo/*/pages/*.html'], { base: "./" })
        .pipe(replace('="images/', '="../../../images/'))
        .pipe(replace('"pages/', '"../pages/'))
        .pipe(replace('href="index.html"', 'href="../index.html"'))
        .pipe(gulp.dest('.'));
    var replacePath3 = gulp.src(['./demo/*/index.html'], { base: "./" })
        .pipe(replace('="images/', '="../../images/'))
        .pipe(gulp.dest('.'));
    return merge(replacePath1, replacePath2, replacePath3);
});

/*sequence for injecting partials and replacing paths*/
gulp.task('inject', gulp.series('injectPartial' , 'injectCommonAssets' , 'injectLayoutStyles', 'replacePath'));

gulp.task('clean:vendors', function () {
    return del([
      'vendors/**/*'
    ]);
});

/* Copy whole folder of some specific node modules that are calling other files internally */
gulp.task('copyRecursiveVendorFiles', function() {
    var mdi= gulp.src(['./node_modules/@mdi/**/*'])
        .pipe(gulp.dest('./vendors/iconfonts/mdi'));
    var fontawesome = gulp.src(['./node_modules/font-awesome/**/*'])
        .pipe(gulp.dest('./vendors/iconfonts/font-awesome'));
    var flagicon = gulp.src(['./node_modules/flag-icon-css/**/*'])
        .pipe(gulp.dest('./vendors/iconfonts/flag-icon-css'));
    var simplelineicon = gulp.src(['./node_modules/simple-line-icons/**/*'])
        .pipe(gulp.dest('./vendors/iconfonts/simple-line-icon'));
    var themifyicon = gulp.src(['./node_modules/ti-icons/**/*'])
        .pipe(gulp.dest('./vendors/iconfonts/ti-icons'));
    var ionrangesliderImages = gulp.src(['./node_modules/ion-rangeslider/img/*'])
        .pipe(gulp.dest('./vendors/img'));
    var summernote = gulp.src(['./node_modules/summernote/dist/**/*'])
        .pipe(gulp.dest('./vendors/summernote/dist'));
    var tinymce = gulp.src(['./node_modules/tinymce/**/*'])
        .pipe(gulp.dest('./vendors/tinymce'));
    var acebuilds = gulp.src(['./node_modules/ace-builds/src-min/**/*'])
        .pipe(gulp.dest('./vendors/ace-builds/src-min'));
    var lightgallery = gulp.src(['./node_modules/lightgallery/dist/**/*'])
        .pipe(gulp.dest('./vendors/lightgallery'));
    var colorpickerImages = gulp.src(['./node_modules/jquery-asColorPicker/dist/images/**/*'])
        .pipe(gulp.dest('./vendors/images'));
    return merge(
        mdi,
        fontawesome, 
        flagicon, 
        simplelineicon, 
        themifyicon, 
        ionrangesliderImages, 
        summernote,
        tinymce,
        acebuilds,
        colorpickerImages
    );
});

/*Building vendor scripts needed for basic template rendering*/
gulp.task('buildBaseVendorScripts', function() {
    return gulp.src([
        './node_modules/jquery/dist/jquery.min.js', 
        './node_modules/popper.js/dist/umd/popper.min.js', 
        './node_modules/bootstrap/dist/js/bootstrap.min.js', 
        './node_modules/perfect-scrollbar/dist/perfect-scrollbar.min.js'
    ])
      .pipe(concat('vendor.bundle.base.js'))
      .pipe(gulp.dest('./vendors/js'));
});

/*Building vendor styles needed for basic template rendering*/
gulp.task('buildBaseVendorStyles', function() {
    return gulp.src(['./node_modules/perfect-scrollbar/css/perfect-scrollbar.css'])
      .pipe(concat('vendor.bundle.base.css'))
      .pipe(gulp.dest('./vendors/css'));
});

/*Building optional vendor scripts for addons*/
gulp.task('buildOptionalVendorScripts', function() {
    return gulp.src([
        'node_modules/chart.js/dist/Chart.min.js', 
        'node_modules/jquery-bar-rating/dist/jquery.barrating.min.js',
        'node_modules/jquery-sparkline/jquery.sparkline.min.js',
        'node_modules/progressbar.js/dist/progressbar.min.js',
        'node_modules/moment/moment.js',
        'node_modules/fullcalendar/dist/fullcalendar.min.js',
        'node_modules/d3/d3.min.js',
        'node_modules/c3/c3.js',
        'node_modules/chartist/dist/chartist.min.js',
        'node_modules/flot/jquery.flot.js',
        'node_modules/flot/jquery.flot.resize.js',
        'node_modules/flot/jquery.flot.categories.js',
        'node_modules/flot/jquery.flot.fillbetween.js',
        'node_modules/flot/jquery.flot.stack.js',
        'node_modules/flot/jquery.flot.pie.js',
        'node_modules/justgage/raphael-2.1.4.min.js',
        'node_modules/justgage/justgage.js',
        'node_modules/morris.js/morris.min.js',
        'node_modules/jquery-tags-input/dist/jquery.tagsinput.min.js',
        'node_modules/progressbar.js/dist/progressbar.min.js',
        'node_modules/inputmask/dist/jquery.inputmask.bundle.js',
        'node_modules/inputmask/dist/inputmask/phone-codes/phone.js',
        'node_modules/inputmask/dist/inputmask/phone-codes/phone-be.js',
        'node_modules/inputmask/dist/inputmask/phone-codes/phone-ru.js',
        'node_modules/inputmask/dist/inputmask/bindings/inputmask.binding.js',
        'node_modules/dropify/dist/js/dropify.min.js',
        'node_modules/dropzone/dist/dropzone.js',
        'node_modules/jquery-file-upload/js/jquery.uploadfile.min.js',
        'node_modules/jquery-asColor/dist/jquery-asColor.min.js',
        'node_modules/jquery-asGradient/dist/jquery-asGradient.min.js',
        'node_modules/jquery-asColorPicker/dist/jquery-asColorPicker.min.js',
        'node_modules/bootstrap-datepicker/dist/js/bootstrap-datepicker.min.js',
        'node_modules/moment/min/moment.min.js',
        'node_modules/x-editable/dist/bootstrap3-editable/js/bootstrap-editable.min.js',
        'node_modules/tempusdominus-bootstrap-4/build/js/tempusdominus-bootstrap-4.js',
        'node_modules/jquery.repeater/jquery.repeater.min.js',
        'node_modules/typeahead.js/dist/typeahead.bundle.min.js',
        'node_modules/select2/dist/js/select2.min.js',
        'node_modules/codemirror/lib/codemirror.js',
        'node_modules/codemirror/mode/javascript/javascript.js',
        'node_modules/codemirror/mode/shell/shell.js',
        'node_modules/quill/dist/quill.min.js',
        'node_modules/simplemde/dist/simplemde.min.js',
        'node_modules/jquery-validation/dist/jquery.validate.min.js',
        'node_modules/bootstrap-maxlength/bootstrap-maxlength.min.js',
        'node_modules/jquery-steps/build/jquery.steps.min.js',
        'node_modules/jquery-validation/dist/jquery.validate.min.js',
        'node_modules/jquery-mapael/js/jquery.mapael.min.js',
        'node_modules/jquery-mapael/js/maps/france_departments.min.js',
        'node_modules/jquery-mapael/js/maps/world_countries.min.js',
        'node_modules/jquery-mapael/js/maps/usa_states.min.js',
        'node_modules/jvectormap/jquery-jvectormap.min.js',
        'node_modules/jvectormap/tests/assets/jquery-jvectormap-world-mill-en.js',
        'node_modules/datatables.net/js/jquery.dataTables.js',
        'node_modules/datatables.net-bs4/js/dataTables.bootstrap4.js',
        'node_modules/jsgrid/dist/jsgrid.min.js',
        'node_modules/owl-carousel-2/owl.carousel.min.js',
        'node_modules/clipboard/dist/clipboard.min.js',
        'node_modules/colcade/colcade.js',
        'node_modules/jquery-contextmenu/dist/jquery.contextMenu.min.js',
        'node_modules/dragula/dist/dragula.min.js',
        'node_modules/jquery-toast-plugin/dist/jquery.toast.min.js',
        'node_modules/twbs-pagination/jquery.twbsPagination.min.js',
        'node_modules/sweetalert/dist/sweetalert.min.js',
        'node_modules/jquery.avgrund/jquery.avgrund.min.js',
        'node_modules/nouislider/distribute/nouislider.min.js',
        'node_modules/ion-rangeslider/js/ion.rangeSlider.min.js',
        'node_modules/pwstabs/assets/jquery.pwstabs.min.js'
    ])
    .pipe(concat('vendor.bundle.addons.js'))
    .pipe(gulp.dest('./vendors/js'));
});

/*Building optional vendor styles for addons*/
gulp.task('buildOptionalVendorStyles', function() {
    return gulp.src([
        'node_modules/jquery-bar-rating/dist/themes/fontawesome-stars.css',
        'node_modules/fullcalendar/dist/fullcalendar.min.css',
        'node_modules/c3/c3.min.css',
        'node_modules/chartist/dist/chartist.min.css',
        'node_modules/morris.js/morris.css',
        'node_modules/jquery-tags-input/dist/jquery.tagsinput.min.css',
        'node_modules/jquery-bar-rating/dist/themes/fontawesome-stars.css',
        'node_modules/jquery-bar-rating/dist/themes/bars-1to10.css',
        'node_modules/jquery-bar-rating/dist/themes/bars-horizontal.css',
        'node_modules/jquery-bar-rating/dist/themes/bars-movie.css',
        'node_modules/jquery-bar-rating/dist/themes/bars-pill.css',
        'node_modules/jquery-bar-rating/dist/themes/bars-reversed.css',
        'node_modules/jquery-bar-rating/dist/themes/bars-square.css',
        'node_modules/jquery-bar-rating/dist/themes/bootstrap-stars.css',
        'node_modules/jquery-bar-rating/dist/themes/css-stars.css',
        'node_modules/jquery-bar-rating/dist/themes/fontawesome-stars-o.css',
        'node_modules/jquery-bar-rating/examples/css/examples.css',
        'node_modules/dropify/dist/css/dropify.min.css',
        'node_modules/jquery-file-upload/css/uploadfile.css',
        'node_modules/tempusdominus-bootstrap-4/build/css/tempusdominus-bootstrap-4.min.css',
        'node_modules/jquery-asColorPicker/dist/css/asColorPicker.min.css',
        'node_modules/bootstrap-datepicker/dist/css/bootstrap-datepicker.min.css',
        'node_modules/x-editable/dist/bootstrap3-editable/css/bootstrap-editable.css',
        'node_modules/select2/dist/css/select2.min.css',
        'node_modules/select2-bootstrap-theme/dist/select2-bootstrap.min.css',
        'node_modules/codemirror/lib/codemirror.css',
        'node_modules/codemirror/theme/ambiance.css',
        'node_modules/dropify/dist/css/dropify.min.css',
        'node_modules/quill/dist/quill.snow.css',
        'node_modules/simplemde/dist/simplemde.min.css',
        'node_modules/jvectormap/jquery-jvectormap.css',
        'node_modules/datatables.net-bs4/css/dataTables.bootstrap4.css',
        'node_modules/jsgrid/dist/jsgrid.min.css',
        'node_modules/jsgrid/dist/jsgrid-theme.min.css',
        'node_modules/owl-carousel-2/assets/owl.carousel.min.css',
        'node_modules/owl-carousel-2/assets/owl.theme.default.min.css',
        'node_modules/jquery-contextmenu/dist/jquery.contextMenu.min.css',
        'node_modules/dragula/dist/dragula.min.css',
        'node_modules/jquery-toast-plugin/dist/jquery.toast.min.css',
        'node_modules/nouislider/distribute/nouislider.min.css',
        'node_modules/ion-rangeslider/css/ion.rangeSlider.css',
        'node_modules/ion-rangeslider/css/ion.rangeSlider.skinFlat.css',
        'node_modules/pwstabs/assets/jquery.pwstabs.min.css'
    ])
    .pipe(concat('vendor.bundle.addons.css'))
    .pipe(gulp.dest('./vendors/css'));
});

/*sequence for building vendor scripts and styles*/
gulp.task('bundleVendors', gulp.series('clean:vendors','copyRecursiveVendorFiles', 'buildBaseVendorStyles','buildBaseVendorScripts', 'buildOptionalVendorStyles', 'buildOptionalVendorScripts'));

gulp.task('default', gulp.series('serve'));
