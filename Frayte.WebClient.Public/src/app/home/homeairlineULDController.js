
angular.module('ngApp.home').controller('homeairlineULDController', function ($scope, $uibModal, $location, $log, $filter) {
    $scope.airlineULDD = {};

    //uib-modal code here
    $scope.adminPopupIn = function (airline) {
        $uibModal.open({
            animation: true,
            templateUrl: 'home/homeairlineULDEdit.tpl.html',
            controller: 'homeairlineULDEditController',
            windowClass: '',
            size: 'lg',
            resolve: {
                adminPopupIn: function () {
                    return airline;
                }
            }
        });
    };

    //end

    $scope.toggleULd = function () {

        if ($scope.search && $scope.airlineULD) {
            var found = $filter('filter')($scope.airlineULD, { code: $scope.search });
            if (found.length) {
                return false;
            }
            else {
                return true;
            }
        }
        return false;

    };

    $scope.airlineULD = [
        { img: 'AAX.png', code: 'AML', baseType: '', classification: 'M-1', classType: '', contour: '', dimension: '317x224x290', usuabledDimension: '306x233x279', doorDimension: '306x279', volume: '18', estimateMaxGross: '6,800', estimateTareWeight: '295', note: '' },
        { img: 'KMA.png', code: 'KMA', baseType: 'P1P', classification: 'A Pen', classType: '3', contour: '', dimension: '317x224x244', usuabledDimension: '284x211x69', doorDimension: 'Unknown', volume: '4.5', estimateMaxGross: '5,030', estimateTareWeight: '610', note: 'Livestock Stall - Sheep/Goat Pens' },
        { img: 'AAA.png', code: 'AAA', baseType: '', classification: 'A2', classType: '', contour: '', dimension: '317x224x205', usuabledDimension: '306x213x194', doorDimension: '306x194', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '215', note: '' },
        { img: 'AAC.png', code: 'AAC', baseType: '', classification: 'A2', classType: '', contour: '', dimension: '317x224x205', usuabledDimension: '306x213x194', doorDimension: '306x194', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '215', note: '' },
       // { img: '30Feet.png', code: '', baseType: '30ft', classification: '', classType: '', contour: '', dimension: '912x244x244', usuabledDimension: '900x233x233', doorDimension: '', volume: '48', estimateMaxGross: '', estimateTareWeight: '', note: 'B747F Nose Door Only' },

      //  { img: '40Feet.png', code: '', baseType: '30ft', classification: '', classType: '', contour: '', dimension: '1,219x244x244', usuabledDimension: '1,208x233x233', doorDimension: '', volume: '65', estimateMaxGross: '', estimateTareWeight: '', note: 'B747F Nose Door Only' },
        { img: '73F Contour Last Position 18.png', code: 'P1P - DC-8-61/63/71/73F Contour Last Position 18', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x158', usuabledDimension: '307x213x147', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'DC-8-61/63/71/73F Contour Last Position 18' },
       { img: '73F Contour Position 1-17.png', code: 'P1P - DC-8-61/63/71/73F General Contour Position 1-17', baseType: '', classification: '', classType: '', contour: '', dimension: '317X224X208', usuabledDimension: '370X213X197', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'DC-8-61/63/71/73F General Contour Position 1-17' },
       { img: 'AAE.png', code: 'AAE', baseType: '', classification: 'A-2', classType: '', contour: '', dimension: '317x224x200', usuabledDimension: '306x213x189', doorDimension: '306x189', volume: '9', estimateMaxGross: '6,030', estimateTareWeight: '205', note: '' },

       { img: 'AAF.png', code: 'AAF', baseType: 'PIP', classification: 'LD-26', classType: '5', contour: '', dimension: '406X223X162', usuabledDimension: '399X213X152', doorDimension: '305X152', volume: '13', estimateMaxGross: '6,030', estimateTareWeight: '250', note: '' },
       { img: 'AAK.png', code: 'AAK', baseType: '', classification: 'LD-9', classType: '', contour: '', dimension: '317x224x163', usuabledDimension: '306x213x152', doorDimension: '292x152', volume: '11', estimateMaxGross: '6,030', estimateTareWeight: '235', note: 'B737F use' },
       { img: 'AAP.png', code: 'AAP', baseType: 'PIP', classification: 'LD-9', classType: '5', contour: '', dimension: '317X224X163', usuabledDimension: '291X206X147', doorDimension: '300X147', volume: '9', estimateMaxGross: '4,620', estimateTareWeight: '215', note: '' },
       { img: 'AAU.png', code: 'AAU', baseType: 'PLP', classification: 'LD-29', classType: '5', contour: '', dimension: '472X224X163', usuabledDimension: '465X213X152', doorDimension: '305X152', volume: '14', estimateMaxGross: '6,030', estimateTareWeight: '265', note: '' },
       { img: 'ABC.png', code: 'ABC', baseType: '', classification: '', classType: '', contour: '', dimension: '274X224X163', usuabledDimension: '263X213X152', doorDimension: '263X163', volume: '8', estimateMaxGross: '3,620', estimateTareWeight: '165', note: '' },

       { img: 'ABN.png', code: 'ABN', baseType: '', classification: 'A1', classType: '', contour: '', dimension: '274X224X176', usuabledDimension: '263X233X165', doorDimension: '', volume: '10', estimateMaxGross: '3,620', estimateTareWeight: '230', note: 'BAe 146 use' },
       { img: 'ABY.png', code: 'AAC', baseType: '', classification: 'A-2', classType: '', contour: '', dimension: '317X224X205', usuabledDimension: '306X213X194', doorDimension: '306X194', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '215', note: '' },
        { img: 'AEP.png', code: 'AEP', baseType: '', classification: '', classType: '', contour: '', dimension: '224X134X163', usuabledDimension: '213X123X152', doorDimension: '', volume: '3.5', estimateMaxGross: '1,810', estimateTareWeight: '129', note: 'BAe 146, B737F & TU-204C' },
       { img: 'AGA.png', code: 'AAC ASE', baseType: '20ft', classification: 'M-6', classType: '1', contour: '', dimension: '606X244X244', usuabledDimension: '589X234X238', doorDimension: '233X227', volume: '33', estimateMaxGross: '11,340', estimateTareWeight: '1,000', note: '' },
        { img: 'AKC.png', code: 'AKC', baseType: '', classification: 'LD-1', classType: '8', contour: '', dimension: '233x153x163', usuabledDimension: '222x142x152', doorDimension: '148x152', volume: '3.5', estimateMaxGross: '1,580', estimateTareWeight: '80', note: '' },

        { img: 'AKE.png', code: 'AKE', baseType: '', classification: 'LD-3', classType: '8', contour: '', dimension: '201x153x163', usuabledDimension: '193x142x152', doorDimension: '145x152', volume: '3', estimateMaxGross: '1,500', estimateTareWeight: '80', note: '' },
       { img: 'AKH.png', code: 'AKH DKH', baseType: '', classification: 'LD-3-45', classType: '8A', contour: '', dimension: '156X153X114', usuabledDimension: '145X142X103', doorDimension: '142X103', volume: '3', estimateMaxGross: '1,130', estimateTareWeight: '75', note: 'A320' },
       { img: 'AKN.png', code: 'AKN', baseType: '', classification: 'LD-3', classType: '8', contour: '', dimension: '201x153x163', usuabledDimension: '193x142x152', doorDimension: '145x152', volume: '8', estimateMaxGross: '1,350', estimateTareWeight: '80', note: 'Forklift' },
       { img: 'ALF.png', code: 'ALF', baseType: '', classification: '6W', classType: '', contour: '', dimension: '406X153X163', usuabledDimension: '395X142X152', doorDimension: '306X151', volume: '6.5', estimateMaxGross: '3,170', estimateTareWeight: '230', note: '' },
       { img: 'ALP.png', code: 'ALP', baseType: 'ALP', classification: 'LD-11', classType: '6', contour: '', dimension: '317X153X163', usuabledDimension: '305X145X155', doorDimension: '305X155', volume: '6', estimateMaxGross: '3,170', estimateTareWeight: '185', note: '' },

       { img: 'AMA.png', code: 'AMA', baseType: 'P6P', classification: 'M1', classType: '2', contour: '', dimension: '317X244X244', usuabledDimension: '307X229X234', doorDimension: '305X229', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '350', note: '' },
       { img: 'AMD.png', code: 'AMD', baseType: 'P6P', classification: 'M-1H', classType: '2', contour: '', dimension: '317X244X300', usuabledDimension: '305X229X290', doorDimension: '307X290', volume: '19', estimateMaxGross: '6,800', estimateTareWeight: '370', note: '' },
        { img: 'AMF.png', code: 'AMF', baseType: '', classification: '', classType: '', contour: '', dimension: '400X244X163', usuabledDimension: '', doorDimension: '', volume: '13', estimateMaxGross: '2,490', estimateTareWeight: '515', note: 'F1 Transport' },
       { img: 'AMJ.png', code: 'AMJ', baseType: '', classification: 'M-1', classType: '', contour: '', dimension: '317x244x244', usuabledDimension: '306x233x233', doorDimension: '270x233', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '305', note: '' },
        { img: 'AML.png', code: 'AMJ', baseType: '', classification: 'M-1', classType: '', contour: '', dimension: '317X224X290', usuabledDimension: '306X233X279', doorDimension: '306X279', volume: '18', estimateMaxGross: '6,800', estimateTareWeight: '295', note: '' },

        { img: 'AMU.png', code: 'AMU', baseType: 'P6P', classification: 'LD-39', classType: '2BG', contour: '', dimension: '472X244X163', usuabledDimension: '462X239X142', doorDimension: '305X152', volume: '13', estimateMaxGross: '5,030', estimateTareWeight: '290', note: '' },
        { img: 'AMX.png', code: 'AMX', baseType: '', classification: 'M-1', classType: '', contour: '', dimension: '317X244X300', usuabledDimension: '306X233X288', doorDimension: '306X288', volume: '20', estimateMaxGross: '6,800', estimateTareWeight: '300', note: '' },
        { img: 'AXY.png', code: 'AXY', baseType: '', classification: 'LD-3-45', classType: '8A', contour: '', dimension: '274X153X195', usuabledDimension: '263X142X184', doorDimension: '259X171', volume: '6.8', estimateMaxGross: '1,043', estimateTareWeight: '128', note: '' },
        { img: 'AYF.png', code: 'AYX', baseType: '', classification: 'M-1', classType: '', contour: 'Fo27', dimension: '203x109x144', usuabledDimension: '192x98x133', doorDimension: '', volume: '2.4', estimateMaxGross: '800', estimateTareWeight: '100', note: 'Fokker 27 use' },
        { img: 'AYK - ATR.png', code: 'AYK', baseType: '', classification: '', classType: '', contour: 'ATR', dimension: '212X109X127', usuabledDimension: '200X98X116', doorDimension: '', volume: '2', estimateMaxGross: '600', estimateTareWeight: '82', note: 'ATR42 & 72 use' },

        { img: 'AYK - Fo27.png', code: 'AYK', baseType: '', classification: '', classType: '', contour: 'Fo27', dimension: '203x109x144', usuabledDimension: '192x98x133', doorDimension: '', volume: '2.4', estimateMaxGross: '800', estimateTareWeight: '100', note: 'Fokker 27 use' },
       { img: 'AYY.png', code: 'AYY', baseType: '', classification: 'Demi', classType: '7', contour: '', dimension: '224x157x200', usuabledDimension: '213x146x188', doorDimension: 'Unknown', volume: '5', estimateMaxGross: '3,010', estimateTareWeight: '80', note: '' },
       { img: 'CF, Position 13-14.png', code: 'P6P - Q6 - MD-11F/CF,Position 13-14', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317X224X244', usuabledDimension: '307X233X233', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'MD-11F/CF, Position 13-14' },
       { img: 'DPE.png', code: 'DPE', baseType: '', classification: 'LD-2', classType: '8D', contour: '', dimension: '157x153x163', usuabledDimension: '145x152x150', doorDimension: '112x152', volume: '2.5', estimateMaxGross: '1,220', estimateTareWeight: '90', note: '' },
       { img: 'DQF.png', code: 'DQF', baseType: '', classification: 'LD-8', classType: '6A', contour: '', dimension: '317x153x163', usuabledDimension: '306x142x149', doorDimension: '234x157', volume: '5.5', estimateMaxGross: '2,450', estimateTareWeight: '125', note: '' },

      { img: 'DQP.png', code: 'DQP', baseType: '', classification: 'LD-8', classType: '6A', contour: '', dimension: '317x153x163', usuabledDimension: '306x142x149', doorDimension: '234x157', volume: '5.5', estimateMaxGross: '2,450', estimateTareWeight: '125', note: '' },
       { img: 'DXF.png', code: 'DXF', baseType: '', classification: '', classType: '', contour: '', dimension: '406x97x153', usuabledDimension: '395x86x142', doorDimension: '', volume: '4.5', estimateMaxGross: '1,990', estimateTareWeight: '100', note: '' },
       { img: 'HAY.png', code: 'HAY', baseType: '', classification: '', classType: '', contour: '', dimension: '317X224X202', usuabledDimension: '', doorDimension: '', volume: '', estimateMaxGross: '2,720', estimateTareWeight: '725', note: 'B737F Compatible' },
       { img: 'HCU-6E.png', code: 'HCU-6E', baseType: '', classification: '', classType: '', contour: '', dimension: '274x224x244', usuabledDimension: '263x213x233', doorDimension: '', volume: '13', estimateMaxGross: '4,530', estimateTareWeight: '150', note: 'Military Pallet' },
      { img: 'HCU-10C.png', code: 'HCU-10C', baseType: '', classification: '', classType: '', contour: '', dimension: '224x137x183 (H11.4)', usuabledDimension: '213x126x172', doorDimension: '', volume: '4.6', estimateMaxGross: '2,268', estimateTareWeight: '97', note: 'Military Pallet(463L)' },

       { img: 'HMA - Droptop.png', code: 'HMA-Droptop', baseType: 'P6P', classification: 'M-1', classType: '', contour: '', dimension: '317X244X242', usuabledDimension: '', doorDimension: '', volume: '', estimateMaxGross: '6,800', estimateTareWeight: '835', note: '' },
       { img: 'HMA.png', code: 'AMA', baseType: 'P6P', classification: 'LD-9', classType: '2', contour: '', dimension: '317x244x238', usuabledDimension: 'Unknown', doorDimension: '233x225', volume: '15', estimateMaxGross: '3,500', estimateTareWeight: '1,310', note: 'Horse Stall' },
       { img: 'HMC.png', code: 'HMC', baseType: '', classification: '', classType: '', contour: '', dimension: '317X244X238', usuabledDimension: '', doorDimension: '', volume: '15', estimateMaxGross: '6,800', estimateTareWeight: '850', note: 'Horse Stall Collapsible' },
       { img: 'HMJ - Droptop.png', code: 'HMJ', baseType: '', classification: '', classType: '', contour: '', dimension: '317X244X233', usuabledDimension: '', doorDimension: '', volume: '14', estimateMaxGross: '1,260', estimateTareWeight: '1,080', note: 'Horse Stall Droptop' },
       { img: 'HMJ.png', code: 'HMJ', baseType: '', classification: '', classType: '', contour: '', dimension: '317X244X233', usuabledDimension: '', doorDimension: '', volume: '14', estimateMaxGross: '6,800', estimateTareWeight: '1,260', note: 'Horse Stall' },

      { img: 'HMLai.png', code: 'HML', baseType: '', classification: 'M-1', classType: '', contour: '', dimension: '317X244X233', usuabledDimension: '', doorDimension: '', volume: '14', estimateMaxGross: '6,800', estimateTareWeight: '1,160', note: 'Horse Stall' },
      { img: 'HMX - Horse Stall Droptop.png', code: 'HMX', baseType: '', classification: '', classType: '', contour: '', dimension: '317X244X244', usuabledDimension: '', doorDimension: '', volume: '15', estimateMaxGross: '6,800', estimateTareWeight: '1,060', note: 'Horse Stall Droptop' },
      { img: 'HXA.png', code: 'HXA', baseType: '', classification: '', classType: '', contour: '', dimension: '317X274X242', usuabledDimension: '', doorDimension: '', volume: '19', estimateMaxGross: '6,800', estimateTareWeight: '1,210', note: 'Horse Stall' },
      { img: 'MD10F 30 Pallet contour (Middle Section).png', code: 'MD10F 30 Pallet contour (Middle Sesction)', baseType: '', classification: 'M-1', classType: '', contour: '', dimension: '274x224x163, 274x224x208, 274x224x244', usuabledDimension: '263x213x152, 263x213x197, 263x213x233', doorDimension: '306x279', volume: '8.5, 11, 13', estimateMaxGross: '4,530', estimateTareWeight: '74', note: 'DC/MD10F, 30 Pallets Contour (Middle Section)' },
      { img: 'MD10F, Position 12.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: 'Q6', dimension: '317x244x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'DC/MD10F,Position 12' },

     { img: 'P1P - A300-600F Side by Side Contour Position 1-9LR.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: 'Q6', dimension: '317X224X224', usuabledDimension: '307x213x233', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300-600F Side Contour Position 1-9L/R' },
     { img: 'P1P - A 300-600F Side by Side Contourt Position 10-12.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: 'Q6', dimension: '317x224x190', usuabledDimension: '307x213x179', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300-600F Side Contour Position 10-12' },
     { img: 'P1P - A300-600F Single Row Contour Position 10-12.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x190', usuabledDimension: '307x213x179', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300-600F Side By Side Contour Position 10-12' },
     { img: 'P1P - A300B4-200F Side by Side Contour Position 10.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x210', usuabledDimension: '307x213x199', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300B4-200F Side by Side Contour Position 10' },

     { img: 'P1P - A300B4-200F Side by Side Contour Position 11.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x190', usuabledDimension: '307x213x179', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300B4-200F Side by SideContour Position 11' },
     { img: 'P1P - A300B4-200F Single Row Contour Position  1 to 11.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x240', usuabledDimension: '307x213x229', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300B4-200F Single RowContour Position 1 to 11' },
     { img: 'P1P - A310F Side by Side Contour Position 1 to 6LR.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x240', usuabledDimension: '307x213x229', doorDimension: '', volume: '13', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300F Side By SideContour Position 1 to 6LR' },
     { img: 'P1P - A 310F Side by Side Contour Position 7LR.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x220', usuabledDimension: '307x213x209', doorDimension: '', volume: '12', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A310F Side By side Contour Position 7LR' },
     { img: 'P1P - A310F Single Row Contour Position 1 to 9.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x240', usuabledDimension: '307x213x229', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A310F Single Row Contour Position 1 to 9' },

     { img: 'P1P - A310F Single Row Contour Position 10.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x220', usuabledDimension: '307x213x209', doorDimension: '', volume: '14', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A310F Single Row Contour Position 10' },
     { img: 'P1P - A310F Single Row Contour Position 11 to 12.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x200', usuabledDimension: '307x213x189', doorDimension: '', volume: '12', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A310F Single Row Contour Position 11 to 12' },
     { img: 'P1P - AN2 Contour Position A-C.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x205', usuabledDimension: '307x213x205', doorDimension: '', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'AN12 Contour Position A-C' },
     { img: 'P1P - AN12 Contour Position D.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x205', usuabledDimension: '307x213x205', doorDimension: '', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'AN12 Contour Position A-C' },
     { img: 'P1P - B707 Freighter Contour.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x208', usuabledDimension: '307x213x197', doorDimension: '', volume: '13', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A707 Freighter Contour' },


     { img: 'P1P - B737 Freighter Contour.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x198', usuabledDimension: '307x213x187', doorDimension: '', volume: '12', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B737 Freighter Contour' },
     { img: 'P1P - B737-700C Convertible Freighter Contour.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x149', usuabledDimension: '307x213x138', doorDimension: '', volume: '9', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B737-700C Convertible Freighte' },
     { img: 'P1P - B757-200F SF-PCF ContourAi.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x208', usuabledDimension: '307x213x197', doorDimension: '', volume: '13', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A757 Freighter & B727F Contour' },
     { img: 'P1P - B767-200F Side by Side Contour Position 1 to 8L-R.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x224', usuabledDimension: '307x213x233', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B767-200F Side By Side Contour position 1 to 8L/R' },

     { img: 'P1P - B767-200F Single Row Contour Position 1 to 11.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x240', usuabledDimension: '307x213x229', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300B4-200F Single Row Contour position 1 to 11' },
     { img: 'P1P - B767-200F Single Row Contour Position 12.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x233', usuabledDimension: '307x213x222', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B767-200F Single Row Contour position 12' },
     { img: 'P1P - B767-300F Contour Position 1 and LAST.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x200', usuabledDimension: '307x213x188', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B767-300F Contour position 1 and LAST' },
     { img: 'P1P - B767-300F Side by Side Contour Position 2 to 12L-R.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x233', usuabledDimension: '307x213x222', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B767-300F Side by Side Contour position 2 to 12L/R' },
     { img: 'P1P - B767-300F Single Row Contour Position 2 to 16.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x224', usuabledDimension: '307x213x233', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B767-300F Single Row Contour position 2 to 16' },

     { img: 'P1P - DC-8-55F Contour Last Position 13.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x208', usuabledDimension: '307x213x147', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'DC-8-55F Contour Last Position 13' },
     { img: 'P1P - IL-62M Freighter Contour.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x185', usuabledDimension: '307x213x174', doorDimension: '', volume: '11', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'IL-62M Freighter Contour' },
     { img: 'P1P - IL-62M Freighter Contour.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x285', usuabledDimension: '307x213x274', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'L 1011F, Position 2L/R' },

     { img: 'P1P - L1011F Contour, Position 1L-Rai.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x285', usuabledDimension: '307x213x274', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'L1011F, Position IL/R' },
     { img: 'P1P - L1011F Contour, Position 3 to 10LR.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x285', usuabledDimension: '307x213x274', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'L1011F,Position 3 to 10L/R' },
     { img: 'P1P - L1011F Contour, Position 11LR.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x285', usuabledDimension: '307x213x274', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'L1011F,Position 11L/R' },

     { img: 'P1P - L1011F Contour, Position 12.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x211', usuabledDimension: '307x213x200', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'L1011F,Position 12' },
     { img: 'P1P - LDP.png', code: 'P1P', baseType: '', classification: 'LD-7', classType: '5', contour: 'LDP', dimension: '317x224x163', usuabledDimension: '307x213x163', doorDimension: '', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '105', note: '' },
     { img: 'P1P - Q6-A330-200Fai.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x224', usuabledDimension: '307x213x233', doorDimension: '', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A330F (SBS)' },
     { img: 'P1P - Q6-DC-MD10F, Position 11LR.png', code: 'P1P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x244', usuabledDimension: '307x213x233', doorDimension: '', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'DC/MD10F,Position 11L/R' },


     { img: 'P1P - Q6.png', code: 'P1P', baseType: '', classification: 'LD-7', classType: '5', contour: 'Q6', dimension: '317x224x244', usuabledDimension: '307x213x233', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: '' },
     { img: 'P1P - Q7.png', code: 'P1P', baseType: '', classification: 'LD-7', classType: '2H', contour: 'Q7', dimension: '317x224x300', usuabledDimension: '307x213x288', doorDimension: '', volume: '18', estimateMaxGross: '6,030', estimateTareWeight: '105', note: '' },
     { img: 'P1P - TU-20AC Contour.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x198', usuabledDimension: '307x213x187', doorDimension: '', volume: '15', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'TU-204C Contour' },
     { img: 'P6P - A1 Position, B747F.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317x244x244', usuabledDimension: '', doorDimension: '', volume: '15', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A1 Position B747F' },
     { img: 'P6P - A1 Position, B747F.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317x244x244', usuabledDimension: '', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A2 Position B747F' },

     { img: 'P6P - A300-600F, Single Row Contour Position 12 to 14.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: 'Q6', dimension: '317x224x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '17', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'A300-600F, Single Row Contour Position 12 to 14' },
     { img: 'P6P - A300B4-200F, Single Row Contour Position 1 to 11.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A300B4-200F, Single Row Contour Position 1 to 11' },
     { img: 'P6P - A300B4-200F, Single Row Contour Position 12 to 13.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x224x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A300B4-200F, Single Row Contour Position 12 to 13' },
     { img: 'P6P - A300B4-200F, Single Row Contour Position 14.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: '', dimension: '317X224X248', usuabledDimension: '307X233X237', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A300B4-200F, Single Row Contour Position 14' },
     { img: 'P6P - A310RF Side by Side Contour Position 1 to 6LR.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x244x240', usuabledDimension: '307x233x229', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A310F Side By Side Contour Position 1 to 6LR' },

     { img: 'P6P - A310 Single Row Position 1 to 8.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x244x240', usuabledDimension: '307x233x229', doorDimension: '', volume: '15', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A310F Single Row Contour Position 1 to 8' },
     { img: 'P6P - A310F Single Row Contour Position 9.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x244x220', usuabledDimension: '307x233x209', doorDimension: '', volume: '14', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A310F Single Row Contour Position 9' },
     { img: 'P6P - A310F Single Row Contour Position 10 to 11.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: '', dimension: '317x244x200', usuabledDimension: '307x233x189', doorDimension: '', volume: '13', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A310F Single Row Contour Position 10 to 11' },
     { img: 'P6P - B767-200F,Single Row Contour Position 1 to 10.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317X244X244', usuabledDimension: '307X233X233', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'B767-200F, Single Row Contour Position 1 to 10' },
     { img: 'P6P- B767-200F,Single Row Contour Position 9.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317x244x233', usuabledDimension: '307x233x222', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'B767-200F, Single Row Contour Position 9' },


     { img: 'P6P - B767-200F,Single Row Contour Position 11.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317x244x233', usuabledDimension: '307x233x222', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'B767-200F, Single Row Contour Position 11' },
     { img: 'P6P - B767-200F,Single Row Contour Position 12.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317x244x205', usuabledDimension: '307x233x194', doorDimension: '', volume: '15', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'B767-200F, Single Row Contour Position 12' },
     { img: 'P6P - B767-300F,Single Row Contour Position 2 to 14.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317x244x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'B767-300F, Single Row Contour Position 2 to 14' },
     { img: 'P6P - LDP.png', code: 'P6P', baseType: 'P6P', classification: 'LD-9', classType: '2BG', contour: 'LDP', dimension: '317x244x163', usuabledDimension: '307x233x152', doorDimension: '', volume: '10', estimateMaxGross: '6,800', estimateTareWeight: '105', note: '' },
     { img: 'P6P - Q6-IL-96-400T Only.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317X244X244', usuabledDimension: '307X233X233', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'Il-96-400T' },


     { img: 'P6P - Q6-IL-96-400T Only2.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317x244x244', usuabledDimension: '307x233x233', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'Il-96-400T' },
     { img: 'P6P - Q6-MD-11F-CF-DC-MD10F, Position 1LR.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317x244x244', usuabledDimension: '307x233x233', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'MD-11F/CF & DC/MD10F Position 1L/R' },
     { img: 'P6P - Q6-MD-11F-CF-DC-MD10F, Position 2-10LR.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317x244x244', usuabledDimension: '307x233x233', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'MD-11F/CF & DC/MD10F, Position 2-10L/R' },
     { img: 'P6P - Q6-MD-11F-CF, Position 11LR.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317x244x244', usuabledDimension: '307x233x233', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'MD-11F/CF, Position 11L/R' },
     { img: 'P6P - Q6-MD-11F-CF, Position 12LR.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317X244X244', usuabledDimension: '307X233X233', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'MD-11F/CF, Position 12L/R' },


     { img: 'P6P - Q6-Side by Side (A to M) Contour - A330-200F.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: 'Q6', dimension: '317x244x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A330F, Single Row' },
     { img: 'P6P - Q6 Side by Side Contour -A330-200F.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: 'Q6', dimension: '317x244x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '14', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A330F (SBS)' },
     { img: 'P6P - Q6 Side Row Contour -A330-200F.png', code: 'P6P', baseType: '', classification: '', classType: '', contour: 'Q6', dimension: '317x244x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A330F, Single Row' },
     { img: 'P6P - Q6.png', code: 'P6P', baseType: 'P6P', classification: 'LD-9', classType: '2BG', contour: 'Q6', dimension: '317x244x244', usuabledDimension: '307x233x233', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: '' },
     { img: 'P6P - Q7-B777F Only.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317X244X294', usuabledDimension: '307X233X283', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'B777F' },


     { img: 'P6P - Q7.png', code: 'P6P', baseType: 'P6P', classification: 'LD-9', classType: '2C', contour: 'Q7', dimension: '317x244x299', usuabledDimension: '307x233x290', doorDimension: '', volume: '21', estimateMaxGross: '6,800', estimateTareWeight: '105', note: '' },
     { img: 'P6P - TU-20AC Contour.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: '', dimension: '317x244x198', usuabledDimension: '307x233x187', doorDimension: '', volume: '13', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'TU-204C Contour' },
     { img: 'P6P - X5 Type A.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'X5-A', dimension: '317x244x163', usuabledDimension: '', doorDimension: '', volume: '13', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'X5 Contour Type A' },
     { img: 'P6P -X5 Type B.png', code: 'P6P', baseType: 'P6P', classification: '', classType: '', contour: 'X5-B', dimension: '317x244x195', usuabledDimension: '', doorDimension: '', volume: '16', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'X5 Contour Type B' },
     { img: 'PAD- With Folding Wings.png', code: 'P6P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317X244X198', usuabledDimension: '307X233X187', doorDimension: '', volume: '13', estimateMaxGross: '5,000', estimateTareWeight: '150', note: 'Folding Wings' },


     { img: 'PAJ -HD.png', code: 'PAJ', baseType: 'P1P', classification: 'LD-7', classType: '5', contour: '', dimension: '317x224x163, 317x224x244', usuabledDimension: '307x213x163, 307x213x233', doorDimension: '', volume: '10, 15', estimateMaxGross: '6,790', estimateTareWeight: '140', note: 'HD' },
     { img: 'PAX.png', code: 'PAX', baseType: 'P1P', classification: 'LD-7', classType: '5', contour: '', dimension: '274x224x163, 274x224x244, 317x224x163, 317x224x244', usuabledDimension: '263x213x152, 263x213x233, 306x213x152, 306x213x233', doorDimension: '8, 13, 10, 15', volume: '', estimateMaxGross: '3,620, 6,800', estimateTareWeight: '110', note: '' },
    //{ img: 'PBJ -DC-9F Pallets Contour.png', code: '', baseType: '', classification: '', classType: '', contour: '', dimension: '274X224X195', usuabledDimension: '263X213X184', doorDimension: '', volume: '10', estimateMaxGross: '4,530', estimateTareWeight: '74', note: 'DC-9F Pallet Contour' },
     { img: 'PBJ - L-100-30 Pallets Contour.png', code: 'PBJ', baseType: '', classification: '', classType: '', contour: '', dimension: '274X224X244', usuabledDimension: '263X213X233', doorDimension: '', volume: '13', estimateMaxGross: '4,530', estimateTareWeight: '74', note: 'L-100-30 Pallets Contour' },
     { img: 'PBJ -L-188F Pallets Contour.png', code: 'PBJ', baseType: '', classification: '', classType: '', contour: '', dimension: '274x224x192', usuabledDimension: '263x213x181', doorDimension: '', volume: '10', estimateMaxGross: '4,530', estimateTareWeight: '75', note: 'L-188F Pallet Contour' },


     //{ img: 'PBJ -LDP.png', code: '', baseType: '', classification: '', classType: '', contour: '', dimension: '274x224x163', usuabledDimension: '263x213x152', doorDimension: '', volume: '8.5', estimateMaxGross: '4,530', estimateTareWeight: '74', note: '' },
     { img: 'PBJ -Main Deck Tail Pallet Contour.png', code: 'PBJ', baseType: '', classification: '', classType: '', contour: '', dimension: '274x224x200', usuabledDimension: '263x213x189', doorDimension: '', volume: '10', estimateMaxGross: '4,530', estimateTareWeight: '75', note: 'IL-96-400T' },
     { img: 'PBJ -TU-1545 Contour.png', code: 'PBJ', baseType: '', classification: '', classType: '', contour: '', dimension: '274X224X137', usuabledDimension: '263X213X126', doorDimension: '', volume: '7', estimateMaxGross: '4,530', estimateTareWeight: '75', note: 'TU-154S Contour' },
    //{ img: 'PBJ.png', code: '', baseType: '', classification: '', classType: '', contour: '', dimension: '274X224X163, 274x213x197, 263x213x233', usuabledDimension: '263X213x152, 263x213x197, 263x213x233', doorDimension: '', volume: '8.5, 11, 13', estimateMaxGross: '4,530', estimateTareWeight: '74', note: '' },
     //{ img: 'PCF Contour Position 15ai.png', code: 'P1P', baseType: 'P1P', classification: '', classType: '', contour: '', dimension: '317x224x195', usuabledDimension: '307x213x184', doorDimension: '', volume: '12', estimateMaxGross: '6,030', estimateTareWeight: '105', note: 'B757-200SF/PCFContour Position 15' },


     //{ img: 'PGA -Q6-B777F.png', code: '', baseType: '20ft', classification: '', classType: '', contour: 'Q6', dimension: '606x244x244', usuabledDimension: '897x233x233', doorDimension: '', volume: '31', estimateMaxGross: '11,340', estimateTareWeight: '500', note: 'B777F' },
     //{ img: 'PGA -Q6.png', code: '', baseType: '20ft', classification: 'M-6', classType: '1', contour: 'Q6', dimension: '606x244x244', usuabledDimension: '597x233x233', doorDimension: '', volume: '33', estimateMaxGross: '11,340', estimateTareWeight: '500', note: '' },
     { img: 'PGA -Q7-B777F.png', code: '20ft', baseType: '', classification: '', classType: '', contour: 'Q7', dimension: '606x244x300', usuabledDimension: '597x233x288', doorDimension: '', volume: '32', estimateMaxGross: '11,340', estimateTareWeight: '500', note: 'B777F' },
     { img: 'PGA -Q7.png', code: '20ft', baseType: '', classification: 'M-6', classType: '1', contour: 'Q7', dimension: '606x244x300', usuabledDimension: '597x233x288', doorDimension: '', volume: '33', estimateMaxGross: '11,340', estimateTareWeight: '500', note: '' },
     { img: 'PKC-1.png', code: 'PKC', baseType: '', classification: '', classType: '', contour: '', dimension: '153x156x162', usuabledDimension: '142x145x151', doorDimension: '', volume: '3', estimateMaxGross: '1,587', estimateTareWeight: '34', note: '' },



     { img: 'PKC.png', code: 'PKC', baseType: '', classification: '', classType: '', contour: '', dimension: '153X156', usuabledDimension: '', doorDimension: '', volume: '', estimateMaxGross: '1,587', estimateTareWeight: '34', note: '' },
     { img: 'PLA.png', code: 'PLA', baseType: '', classification: '', classType: '6', contour: '', dimension: '317X153X163', usuabledDimension: '306X142X152', doorDimension: '', volume: '6', estimateMaxGross: '3,170', estimateTareWeight: '85', note: '' },
     //{ img: 'PQA.png', code: '', baseType: '', classification: 'HP', classType: '8', contour: 'LDP', dimension: '244x153x163', usuabledDimension: '233x142x152', doorDimension: '', volume: '5', estimateMaxGross: '2,440', estimateTareWeight: '100', note: 'B767 Use' },

     { img: 'PRA - 116 Inch Height.png', code: '16ft', baseType: '', classification: '', classType: '', contour: '116', dimension: '498x244x294', usuabledDimension: '487x233x283', doorDimension: '', volume: '32', estimateMaxGross: '11,300', estimateTareWeight: '410', note: '' },
     { img: 'PRA - 118 Inch Height.png', code: '16ft', baseType: '', classification: '', classType: '', contour: '118', dimension: '498x244x300', usuabledDimension: '487x233x289', doorDimension: '', volume: '32', estimateMaxGross: '11,300', estimateTareWeight: '410', note: '' },




    { img: 'PRA.png', code: '16ft', baseType: '', classification: 'MDP', classType: '1P', contour: '', dimension: '498x244x244', usuabledDimension: '487x233x233', doorDimension: '', volume: '26', estimateMaxGross: '11,300', estimateTareWeight: '410', note: '' },
     { img: 'PYB.png', code: 'PLA', baseType: 'P1P', classification: '', classType: '6', contour: '', dimension: '317x153x163', usuabledDimension: '306x142x152', doorDimension: '', volume: '6', estimateMaxGross: '3,170', estimateTareWeight: '85', note: '' },
    { img: 'RAP.png', code: 'RAP', baseType: 'P1P', classification: 'LD-9', classType: '5', contour: '', dimension: '317x244x163', usuabledDimension: '308x208x147', doorDimension: '216x147', volume: '9', estimateMaxGross: '4,620', estimateTareWeight: '400', note: 'Reefer' },
     { img: 'RAU.png', code: 'RAU', baseType: 'P1P', classification: 'LD-29', classType: '5', contour: '', dimension: '472x224x163', usuabledDimension: '461x213x152', doorDimension: '', volume: '10', estimateMaxGross: '6,030', estimateTareWeight: '450', note: 'Cool Container' },
     { img: 'RKN.png', code: 'RKN EVN', baseType: '', classification: 'LD-3', classType: '8', contour: '', dimension: '201x153x163', usuabledDimension: '158x145x140', doorDimension: '', volume: '3', estimateMaxGross: '1,580', estimateTareWeight: '210', note: 'Reefer' },


     //{ img: 'RP6P -A300-600F, Single Row Contour Position 1 to 11LR.png', code: '', baseType: 'P6P', classification: '', classType: '', contour: 'Q6', dimension: '317x244x248', usuabledDimension: '307x233x237', doorDimension: '', volume: '17', estimateMaxGross: '6,800', estimateTareWeight: '105', note: 'A300-600F, Single RowContour Position 1 to 11L/R' },
     { img: 'SAA.png', code: 'SAA', baseType: '', classification: '', classType: '', contour: '', dimension: '274x224x163', usuabledDimension: '263x213x152', doorDimension: '263x163', volume: '8', estimateMaxGross: '3,620', estimateTareWeight: '165', note: '' },
     { img: 'VRA.png', code: 'VRA', baseType: '16ft', classification: 'M-6', classType: '1P', contour: '', dimension: '498x244x244, 606x244x244', usuabledDimension: '487x233x233, 595x233x233', doorDimension: '', volume: '26', estimateMaxGross: '8,900', estimateTareWeight: '400', note: 'Twin Car Racks is able to load into Pallets; PRA PMA P4A P4M PZA 20 Feet PGA PGE PGF PSAPSG P7E P7A P7FP7G' },
     { img: 'XAW -With Fixed Wings.png', code: 'XAW', baseType: '', classification: 'LD-7', classType: '5', contour: '', dimension: '406x224x163', usuabledDimension: '395x213x152', doorDimension: '', volume: '12', estimateMaxGross: '5,000', estimateTareWeight: '170', note: 'Fixed Angle Wings' }

    ];

});