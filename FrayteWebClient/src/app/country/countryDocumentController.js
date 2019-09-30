/**
 * Controller
 */
angular.module('ngApp.country').controller('CountryDocumentController', function ($scope, $location, CountryService, $translate, SessionService, $uibModal, $uibModalInstance, countryDetail, countryDocument, $log, toaster) {

    //Set Multilingual for Modal Popup
    var setModalOptions = function () {
        $translate(['FrayteValidation', 'PleaseCorrectValidationErrors']).then(function (translations) {

            $scope.TitleFrayteValidation = translations.FrayteValidation;
            $scope.TextValidation = translations.PleaseCorrectValidationErrors;

        });
    };

    $scope.DocumentTypes = [
       { id: 1, name: 'Air' },
       { id: 2, name: 'Courier' },
       { id: 3, name: 'Expryes' },
       { id: 4, name: 'Sea' }
    ];

    $scope.countryDetail = countryDetail;

    $scope.documentDetail = {
        CountryDocumentId: countryDocument.CountryDocumentId,
        DocumentName: countryDocument.DocumentName,
        ShipmentType: countryDocument.ShipmentType,
        CountryId: countryDocument.CountryId
    };

    $scope.SaveContryDocument = function (isValid, documentDetail) {
        if (isValid) {
            var countryDocumentId = documentDetail.CountryDocumentId;

            //To Dos: Write code here to inject the new date in ui grid datasource
            if (countryDocumentId === undefined || countryDocumentId === 0) {
                if ($scope.countryDetail.CountryDocuments === null || $scope.countryDetail.CountryDocuments === undefined) {
                    $scope.countryDetail.CountryDocuments = [];
                    $scope.countryDetail.CountryDocuments.push(documentDetail);
                }
                else {
                    $scope.countryDetail.CountryDocuments.push(documentDetail);
                }
            }
            else {
                //Need to update the countryDocuments collection and then return back to main grid
                $scope.updateCountryDocument(documentDetail);
            }

            $uibModalInstance.close($scope.countryDetail.CountryDocuments);
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

    $scope.cancel = function () {
        $uibModalInstance.dismiss($scope.countryDetail.CountryDocuments);
    };

    $scope.updateCountryDocument = function (documentDetail) {
        var objects = $scope.countryDetail.CountryDocuments;

        for (var i = 0; i < objects.length; i++) {
            if (objects[i].CountryDocumentId === documentDetail.CountryDocumentId) {
                objects[i] = documentDetail;
                break;
            }
        }
    };


    function init() {
        // set Multilingual Modal Popup Options
        setModalOptions();
    }

    init();
});