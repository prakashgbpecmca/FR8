/// <reference path="vendor/ng-file-upload/ng-file-upload-all.js" />
/// <reference path="vendor/ng-file-upload/ng-file-upload-all.js" />
/**
 * This file/module contains all configuration for the build process.
 */
module.exports = {
    /**
     * The `build_dir` folder is where our projects are compiled during
     * development and the `compile_dir` folder is where our app resides once it's
     * completely built.
     */
    build_dir: 'build',
    compile_dir: 'bin',

    /**
     * This is a collection of file patterns that refer to our app code (the
     * stuff in `src/`). These file paths are used in the configuration of
     * build tasks. `js` is all project javascript, less tests. `ctpl` contains
     * our reusable components' (`src/common`) template HTML files, while
     * `atpl` contains the same, but for our app's code. `html` is just our
     * main HTML file, `less` is our main stylesheet, and `unit` contains our
     * app's unit tests.
     */
    app_files: {
        js: ['src/**/*.js', '!src/**/*.spec.js', '!src/assets/**/*.js'],

        atpl: ['src/app/**/*.tpl.html'],
        ctpl: ['src/common/**/*.tpl.html', 'src/common/*.tpl.html'],

        html: ['src/index.html'],
        less: ['src/less/*.less', 'src/app/**/*.less']
    },

    /**
     * This is the same as `app_files`, except it contains patterns that
     * reference vendor code (`vendor/`) that we need to place into the build
     * process somewhere. While the `app_files` property ensures all
     * standardized files are collected for compilation, it is the user's job
     * to ensure non-standardized (i.e. vendor-related) files are handled
     * appropriately in `vendor_files.js`.
     *
     * The `vendor_files.js` property holds files to be automatically
     * concatenated and minified with our project source files.
     *
     * The `vendor_files.css` property holds any CSS files to be automatically
     * included in our app.
     *
     * The `vendor_files.assets` property holds any assets to be copied along
     * with our app's assets. This structure is flattened, so it is not
     * recommended that you use wildcards.
     */
    vendor_files: {
        js: [
        'vendor/angular-google-map/lodash.js',
            'vendor/angular-bootstrap/jquery-1.11.3.js',
        'vendor/angular/angular.min.js',
        'vendor/angular-animate/angular-animate.js',
         'vendor/angular-sanitize/angular-sanitize.min.js', 
        'vendor/angular-ui-router/angular-ui-router.min.js',
        'vendor/angular-bootstrap/bootstrap.min.js',
        'vendor/angular-bootstrap/ui.bootstrap-tpls-2.1.4.js',
        'vendor/angular-ui-utils/ui-utils.min.js',
        'vendor/angular-ui-grid/ui-grid.min.js',
        'vendor/loading-bar/loading-bar.min.js',
        'vendor/angular-toaster/toaster.js',
        'vendor/moment/moment.js',
        'vendor/ng-file-upload/ng-file-upload-shim.js',
        'vendor/ng-file-upload/ng-file-upload.js',
        'vendor/ui-mask/mask.js',
        'vendor/angular-google-map/angular-simple-logger.js',
        'vendor/angular-google-map/angular-google-maps.js',
        'vendor/html2canvas/html2canvas.js',
        'vendor/angular-translate/angular-translate.min.js',
        'vendor/angular-translate-staticfiles/angular-translate-loader-static-files.min.js',
        'vendor/df-tab-menu/df-tab-menu.js',
        'vendor/textAngular-rangy/textAngular-rangy.min.js',
         'vendor/textAngular/textAngular.min.js',
         'vendor/angular-bootstrap-colorpicker/bootstrap-colorpicker-module.js',
            'vendor/spin/spin.js'
        ],
        css: [
            'vendor/bootstrap/bootstrap.css',
            'vendor/font-awesome/font-awesome.css',
            'vendor/loading-bar/loading-bar.min.css',
            'vendor/angular-ui-grid/ui-grid-unstable.css',
            'vendor/angular-toaster/toaster.css',
            'vendor/df-tab-menu/df-tab-menu.css',
            'vendor/textAngular/style.css',
            'vendor/angular-bootstrap-colorpicker/colorpicker.css'
        ],
        assets: [
          'vendor/images/*.*'
        ],
        fonts: [
          'vendor/bootstrap/fonts/*.*',
          'vendor/frayte-font/*.*',
          'vendor/font-awesome/fonts/*.*',
          'vendor/angular-ui-grid/fonts/*.*'
        ],
        downloadfiles: [
         'downloadform/*.*'
        ],
        languagesfiles: [
         'src/*.*'
        ],

        json: [
            'vendor/multilungual/*.*'
        ]
    },
};
