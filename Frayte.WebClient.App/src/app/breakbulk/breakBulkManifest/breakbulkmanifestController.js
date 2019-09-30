angular.module('ngApp.breakBulk').controller("BreakbulkManifest", function ($scope, $uibModal, ModalService, config) {


    //date code
    $scope.today = function () {
        $scope.dt = new Date();
    };
    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };

    $scope.inlineOptions = {
        customClass: getDayClass,
        minDate: new Date(),
        showWeeks: true
    };

    $scope.dateOptions = {
        dateDisabled: disabled,
        formatYear: 'yy',
        maxDate: new Date(2020, 5, 22),
        minDate: new Date(),
        startingDay: 1
    };

    // Disable weekend selection
    function disabled(data) {
        var date = data.date,
            mode = data.mode;
        return mode === 'day' && (date.getDay() === 0 || date.getDay() === 6);
    }

    $scope.toggleMin = function () {
        $scope.inlineOptions.minDate = $scope.inlineOptions.minDate ? null : new Date();
        $scope.dateOptions.minDate = $scope.inlineOptions.minDate;
    };

    $scope.toggleMin();

    $scope.open1 = function () {
        $scope.popup1.opened = true;
    };

    $scope.open2 = function () {
        $scope.popup2.opened = true;
    };

    $scope.setDate = function (year, month, day) {
        $scope.dt = new Date(year, month, day);
    };

    $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];
    $scope.altInputFormats = ['M!/d!/yyyy'];

    $scope.popup1 = {
        opened: false
    };

    $scope.popup2 = {
        opened: false
    };

    var tomorrow = new Date();
    tomorrow.setDate(tomorrow.getDate() + 1);
    var afterTomorrow = new Date();
    afterTomorrow.setDate(tomorrow.getDate() + 1);
    $scope.events = [
        {
            date: tomorrow,
            status: 'full'
        },
        {
            date: afterTomorrow,
            status: 'partially'
        }
    ];

    function getDayClass(data) {
        var date = data.date,
            mode = data.mode;
        if (mode === 'day') {
            var dayToCheck = new Date(date).setHours(0, 0, 0, 0);

            for (var i = 0; i < $scope.events.length; i++) {
                var currentDay = new Date($scope.events[i].date).setHours(0, 0, 0, 0);

                if (dayToCheck === currentDay) {
                    return $scope.events[i].status;
                }
            }
        }

        return '';
    }
    //end


    //breakbulk manifest json code
    $scope.breakbulkManifests = [
        { id: 1, manifestNo: 'MNLHR-18090065', dispatchFactory:'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8745', totalWeightShipment: '24 kgs/5 shipments', manifestDate: '27-May-2019', active: false },
        { id: 2, manifestNo: 'MNLHR-18090066', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8746', totalWeightShipment: '4 kgs/5 shipments', manifestDate: '16-May-2019', active: false },
        { id: 3, manifestNo: 'MNLHR-18090067', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8747', totalWeightShipment: '14 kgs/5 shipments', manifestDate: '1-May-2019', active: false },
        { id: 4, manifestNo: 'MNLHR-18090068', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8748', totalWeightShipment: '24 kgs/1 shipments', manifestDate: '28-Apr-2019', active: false },
        { id: 5, manifestNo: 'MNLHR-18090069', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8749', totalWeightShipment: '4 kgs/15 shipments', manifestDate: '18-Apr-2019', active: false },
        { id: 6, manifestNo: 'MNLHR-18090070', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8750', totalWeightShipment: '4 kgs/0 shipments', manifestDate: '2-Apr-2019', active: false },
        { id: 7, manifestNo: 'MNLHR-18090071', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8751', totalWeightShipment: '2 kgs/5 shipments', manifestDate: '31-Mar-2019', active: false },
        { id: 8, manifestNo: 'MNLHR-18090072', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8752', totalWeightShipment: '8 kgs/15 shipments', manifestDate: '1-Mar-2019', active: false },
        { id: 9, manifestNo: 'MNLHR-18090073', dispatchFactory: 'Phone Pvt Ltd.',customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8753', totalWeightShipment: '2 kgs/5 shipments', manifestDate: '26-Feb-2019', active: false },
        { id: 10, manifestNo: 'MNLHR-18090074', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8754', totalWeightShipment: '1 kgs/0 shipments', manifestDate: '5-Feb-2019', active: false },
        { id: 11, manifestNo: 'MNLHR-18090075', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8755', totalWeightShipment: '3 kgs/3 shipments', manifestDate: '30-Jan-2019', active: false },
        { id: 12, manifestNo: 'MNLHR-18090076', dispatchFactory: 'Phone Pvt Ltd.', customer: 'SERIOUS STUFF LIMITED', mawb: '549 2589 8756', totalWeightShipment: '5 kgs/11 shipments', manifestDate: '8-Jan-2019', active: false }
    ];
    //end
 
    //breakbulk purchase order detail code
    $scope.breakbulkViewManifest = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'breakbulkViewManifestController',
            templateUrl: 'breakbulk/breakBulkManifest/breakbulkViewManifest.tpl.html',
            keyboard: true,
            windowClass: 'CustomerAddress-Edit',
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    //breakbulk purchase order detail code
    $scope.breakbulkAddManifest = function () {
        ModalInstance = $uibModal.open({
            Animation: true,
            controller: 'BreakBulkAddManifestController',
            templateUrl: 'breakbulk/breakBulkManifest/breakBulkAddManifest/breakbulkAddManifest.tpl.html',
            keyboard: true, 
            size: 'lg',
            backdrop: 'static'
        });
    };
    //end

    function init() {
        $scope.ImagePath = config.BUILD_URL; 
    }
    init();

});
