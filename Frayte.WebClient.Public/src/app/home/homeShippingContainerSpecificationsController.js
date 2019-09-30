
angular.module('ngApp.home').controller('homeShippingContainerSpecificationsController', function ($scope) {
    $scope.shippingContainer = {};
    $scope.ComingSoon = 'Coming soon...';
    $scope.headerImage = 'Shipping_Contianers-01.png';

    $scope.shippingContainer = [
       { img: 'Shipping_Contianers-02.png', interiorOpening: 'L:5,898 mm W:2,350 mm H:2,390 mm', doorOpening: 'W:2,340 mm H:2,280 mm', top: '', tareWeight: '2,200 Kg', cubicCapacity: '33,0 cbm.', payLoad: '24,800 Kg' },
       { img: 'Shipping_Contianers-03.png', interiorOpening: 'L:12,035 mm W:2,350 mm H:2,373 mm', doorOpening: 'W:2,339 mm H:2,274 mm', top: '', tareWeight: '3,700 kg', cubicCapacity: '67,0 cbm.', payLoad: '28,800 Kg' },
       { img: 'Shipping_Contianers-04.png', interiorOpening: 'L:12,030 mm W:2,350 mm H:2,579 mm', doorOpening: 'W:2,340 mm H:2,690 mm', top: '', tareWeight: '3,930 Kg', cubicCapacity: '76,0 cbm.', payLoad: '28,570 Kg' },
       { img: 'Shipping_Contianers-05.png', interiorOpening: 'L:5,440 mm W:2,294 mm H:2,238 mm', doorOpening: 'W:2,286 mm H:2,237 mm', top: '', tareWeight: '2,750 Kg', cubicCapacity: '27,9 cbm.', payLoad: '24,250 Kg' },


       { img: 'Shipping_Contianers-06.png', interiorOpening: 'L:11,577 mm W:2,294 mm H:2,210 mm', doorOpening: 'W:2,286 mm H:2,238 mm', top: '', tareWeight: '3,950 Kg', cubicCapacity: '58,7 cbm.', payLoad: '28,550 Kg' },
       { img: 'Shipping_Contianers-07.png', interiorOpening: 'L:5,893 mm W:2,346 mm H:2,353 mm', doorOpening: 'W:2,338 mm H:2,273 mm', top: 'L:5,488 mm, W:2,230 mm', tareWeight: '2,200 Kg', cubicCapacity: '32,0 cbm.', payLoad: '28,280 Kg' },
       { img: 'Shipping_Contianers-08.png', interiorOpening: 'L:12,056 mm W:2,347 mm H:2,379 mm', doorOpening: 'W:2,343 mm L:2,279 mm', top: 'L:11,622 mm, W:2,279 mm', tareWeight: '3,800 Kg', cubicCapacity: '67,0 cbm.', payLoad: '28,700 Kg' },
       { img: 'Shipping_Contianers-09.png', interiorOpening: 'L:5,935 mm W:2,398 mm H:2,327 mm', doorOpening: '', top: '', tareWeight: '2,560 Kg', cubicCapacity: '', payLoad: '21,440 Kg' },

       { img: 'Shipping_Contianers-10.png', interiorOpening: 'L:12,080 mm W:2,420 mm H:2,103 mm', doorOpening: '', top: '', tareWeight: '5,480 Kg', cubicCapacity: '', payLoad: '25,000 Kg' },
       { img: 'Shipping_Contianers-11.png', interiorOpening: 'L:13,556 mm W:2,426 mm H:2,697 mm', doorOpening: 'W:2,416 mm H:2,585 mm', top: '', tareWeight: '4,180 Kg', cubicCapacity: '89,0 cbm.', payLoad: '29,820 Kg' },
       { img: 'Shipping_Contianers-12.png', interiorOpening: 'L:13,561 mm W:2,450 mm H:2,459 mm', doorOpening: 'W:2,450 mm H:2,436 mm', top: '', tareWeight: '5,870 Kg', cubicCapacity: '84 cbm.', payLoad: '28,130 Kg' },
       { img: 'Shipping_Contianers-13.png', interiorOpening: 'L:13,102 mm W:2,286 mm H:2,509 mm', doorOpening: 'W:2,294 mm H:2,535 mm', top: '', tareWeight: '5,200 Kg', cubicCapacity: '75.4 cbm.', payLoad: '27,300 Kg' }
    ];

});