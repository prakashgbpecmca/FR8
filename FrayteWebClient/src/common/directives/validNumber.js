angular.module('ngApp.common')
              .directive('validNumber', function () {
                  return {
                      require: '?ngModel',
                      link: function (scope, element, attrs, ngModelCtrl) {

                          element.on('keydown', function (event) {
                              var keyCode = [];
                              if (attrs.allowNegative == "true") {
                                  keyCode = [8, 9, 36, 35, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 109, 110, 173, 190, 189];
                              }
                              else {
                                  keyCode = [8, 9, 36, 35, 37, 39, 46, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 110, 173, 190];
                              }

                              if (attrs.allowDecimal == "false") {
                                  var index = keyCode.indexOf(190);
                                  var index1 = keyCode.indexOf(110);
                                  if (index > -1) {
                                      keyCode.splice(index, 1);
                                  }
                                  if (index1 > -1) {
                                      keyCode.splice(index1, 1);
                                  }
                              }

                              if (keyCode.indexOf(event.which) < 0) {
                                  event.preventDefault();
                              }
                              else {
                                  var oVal = ngModelCtrl.$modelValue || '';
                                  if ([109, 173].indexOf(event.which) > -1 && oVal.indexOf('-') > -1) {
                                      event.preventDefault();
                                  }
                                  else if ([110, 190].indexOf(event.which) > -1 && oVal.indexOf('.') > -1) {
                                      event.preventDefault();
                                  }
                              }

                              // do not remove comment

                              //if ($.inArray(event.which, keyCode) == -1) {
                              //    event.preventDefault();
                              //}
                              //else {
                              //    //    console.log(2);
                              //    var oVal = ngModelCtrl.$modelValue || '';
                              //    if ($.inArray(event.which, [109, 173]) > -1 && oVal.indexOf('-') > -1) {
                              //        event.preventDefault();
                              //    }
                              //    else if ($.inArray(event.which, [110, 190]) > -1 && oVal.indexOf('.') > -1) {
                              //        event.preventDefault();
                              //    }
                              //}
                          });
                          // do not remove comment
                          //.on('blur', function () {
                          //    var fixedValue = parseFloat(0);
                          //    if (element.val() === '' || parseFloat(element.val()) === 0.0 || element.val() === '-') {
                          //        ngModelCtrl.$setViewValue('0.00');
                          //    }
                          //    else if (attrs.allowDecimal == "false") {
                          //        ngModelCtrl.$setViewValue(element.val());
                          //    }
                          //    else {
                          //        if (attrs.decimalUpto) {
                          //            fixedValue = parseFloat(element.val()).toFixed(attrs.decimalUpto);
                          //        }
                          //        else {
                          //            fixedValue = parseFloat(element.val()).toFixed(2);
                          //            ngModelCtrl.$setViewValue(fixedValue);
                          //        }
                          //    }



                          //    ngModelCtrl.$render();
                          //    scope.$apply();
                          //});

                          ngModelCtrl.$parsers.push(function (text) {
                              var oVal = ngModelCtrl.$modelValue;
                              var nVal = ngModelCtrl.$viewValue;
                              //  console.log(nVal);
                              var as = parseFloat(nVal);
                              if (parseFloat(nVal) != nVal) {
                                  if (nVal === null || nVal === undefined || nVal === '' || nVal === '-') {
                                      oVal = nVal;
                                  }
                                  
                                  ngModelCtrl.$setViewValue(oVal);
                                  ngModelCtrl.$render();
                                  return oVal;
                              }
                              else {
                                  var decimalCheck = nVal.split('.');
                                  if (!angular.isUndefined(decimalCheck[1])) {
                                      if (attrs.decimalUpto) {
                                          decimalCheck[1] = decimalCheck[1].slice(0, attrs.decimalUpto);
                                      }
                                      else {
                                          decimalCheck[1] = decimalCheck[1].slice(0, 2);
                                          nVal = decimalCheck[0] + '.' + decimalCheck[1];
                                      }
                                  }
                                  if (parseFloat(nVal) > 1500) {
                                      nVal = oVal;
                                  }
                                  ngModelCtrl.$setViewValue(nVal);
                                  ngModelCtrl.$render();
                                  return nVal;
                              }
                          });

                          ngModelCtrl.$formatters.push(function (text) {
                              if (text === '0' || text == null && attrs.allowDecimal === "false") { return ''; }
                                  //else if (text === '0' || text === null && attrs.allowDecimal !== "false" && attrs.decimalUpto === undefined) {return '0.00';}
                                  //else if (text === '0' || text === null && attrs.allowDecimal !== "false" && attrs.decimalUpto !== undefined) {return parseFloat(0).toFixed(attrs.decimalUpto);}
                                  //else if (attrs.allowDecimal !== "false" && attrs.decimalUpto !== undefined) {return parseFloat(text).toFixed(attrs.decimalUpto);}
                              else { return parseFloat(text); }
                          });
                      }
                  };
              });
