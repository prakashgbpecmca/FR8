/**
 * Controller
 */
angular.module('ngApp.reportSetting').controller('ReportSettingController', function ($scope, toaster, $translate, ReportSettingService) {

    var setModalOptions = function () {
        $translate(['Address', 'Book', 'DeleteHeader', 'DeleteBody', 'Tradelane', 'SuccessfullySavedInformation', 'FrayteInformation', 'FrayteValidation', 'PleaseCorrectValidationErrors', 'FrayteError', 'ErrorSavingRecord', 'ErrorGetting', 'report','setting', 'detail']).then(function (translations) {
            $scope.headerTextOtherAddress = translations.Address + " " + translations.Book + " " + translations.DeleteHeader;
            $scope.bodyTextOtherAddress = translations.DeleteBody + " " + translations.Address;
            $scope.headerTextTradeLane = translations.Tradelane + " " + translations.DeleteHeader;
            $scope.bodyTextTradeLane = translations.DeleteBody + " " + translations.Tradelane + " " + translations.detail;

            $scope.TitleFrayteInformation = translations.FrayteInformation;
            $scope.TextSuccessfullySavedInformation = translations.SuccessfullySavedInformation;

            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;

            $scope.TitleFrayteError = translations.FrayteError;
            $scope.TextSavingError = translations.ErrorSavingRecord;

            $scope.TextErrorGetting = translations.ErrorGetting + " " + translations.report + " " + translations.setting +" "+ translations.detail;

        });
    };

    $scope.reportSettingJson = function () {
        $scope.frayteReportSetting = {
            ReportSettingId: 0,
            IsActive: false,
            ReportTime: '',
            ReportSchedule: null,
            LastRun: null,
            ToMail: null,
            fryateReportSettingEmailDetail: [{
                ReportSettingMailID: 0,
                ReportSettingId: 0,
                CcMail: null
            }]
        };
    };

    $scope.scheduleTypes = [
        {
            value: 'Daily'
        },
        {
            value: 'Weekly'
        },
         {
             value: '15 Days'
         },
        {
            value: 'Monthly'
        }
    ];

    $scope.addEmailCC = function () {
        var data = {
            ReportSettingMailID: 0,
            ReportSettingId: $scope.frayteReportSetting.ReportSettingId,
            CcMail: null
        };

        $scope.frayteReportSetting.fryateReportSettingEmailDetail.push(data);
        
    };
    $scope.saveReportSettingDetail = function (IsValid, frayteReportSetting) {
        if (IsValid) {
            if (frayteReportSetting !== undefined && frayteReportSetting !== null) {
              
                ReportSettingService.SaveReportSetting(frayteReportSetting).then(function (response) {
                    toaster.pop({
                        type: 'success',
                        title: $scope.TitleFrayteInformation,
                        body: $scope.TextSuccessfullySavedInformation,
                        showCloseButton: true
                    });
                }, function () {
                    toaster.pop({
                        type: 'error',
                        title: $scope.TitleFrayteError,
                        body: $scope.TextSavingError,
                        showCloseButton: true
                    });
                });
            }

        }
        else {
            toaster.pop({
                type: 'warning',
                title: $scope.TitleFrayteValidation,
                body: $scope.TextValidation,
                showCloseButton: true
            });
        }


    };

    var getReportSettingDetails = function () {
        ReportSettingService.GetReportSettingDetails().then(function (response) {
            if (response.data !== null) {
                $scope.frayteReportSetting = response.data;
            }
            else {
                $scope.reportSettingJson();
            }

        },
        function () {
            toaster.pop({
                type: 'error',
                title: $scope.TitleFrayteError,
                body: $scope.TextErrorGetting,
                showCloseButton: true
            });
            $scope.reportSettingJson();
        });
    };

    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
        getReportSettingDetails();
    }

    init();

});