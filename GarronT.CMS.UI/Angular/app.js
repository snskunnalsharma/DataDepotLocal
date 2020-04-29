/// <reference path="views/Form/Index.html" />
//'use strict';

var angularApp = angular.module('GarronTCMC-App', ['ngRoute', 'ui.bootstrap', 'ngDialog', 'mgcrea.ngStrap.typeahead', 'as.sortable']);

angularApp.config(function ($locationProvider, $routeProvider, $controllerProvider) {


    var projectPath = $("base").first().attr("href");      // use this path in development mode

    debugger;

    // var projectPath = "/Client/CMS/";                    // use this path in production mode

    var baseSiteUrlPath = projectPath;

    var baseTemplateUrl = baseSiteUrlPath + "Angular/views/";

    var currentpath = window.location.pathname;

    var urlCheckUserPermission = $('#urlCheckUserPermission').val();


   
    $controllerProvider.allowGlobals();

    $routeProvider
        .when(baseSiteUrlPath + 'FormManagement', {
            templateUrl: baseTemplateUrl + 'Form/Index.html',
            controller: 'FormManagementCtrl'
        })
         .when(baseSiteUrlPath + 'FormManagement/Add', {
             templateUrl: baseTemplateUrl + 'Form/AddForm.html',
             controller: 'FormAddctrl',
             //resolve: {
             //    pageData: function ($http, $route) {
             //        return $http.get(urlCheckUserPermission+"/ADD").then(function (response) {
             //            debugger;
             //            debugger;
             //            if (response.data.success === false) {
             //                var $toast = toastr["error"](response.data.returnMessage, "Notification !");

             //                var indexpath = baseSiteUrlPath + "FormManagement";
             //                window.location.hash = indexpath;
             //                //window.location.href = indexpath;
             //               // $location.path(indexpath);
             //                return false;
             //            }
             //        })
             //    }
             //}

         })
         .when(baseSiteUrlPath + 'FormManagement/Edit/:param', {
             templateUrl: baseTemplateUrl + 'Form/AddForm.html',
             controller: 'FormAddctrl',
             paramExample: 'EditParam'
         })
         .when(baseSiteUrlPath + 'FormManagement/View/:param', {
             templateUrl: baseTemplateUrl + 'Form/AddForm.html',
             controller: 'FormAddctrl',
             paramExample: 'ViewParam'
         })
        .when(baseSiteUrlPath + 'FormManagement/Clone', {
            templateUrl: baseTemplateUrl + 'Form/AddForm.html',
            controller: 'FormAddctrl',
            paramExample: 'CloneParam'
        })

    .otherwise({
        redirectTo: baseSiteUrlPath + 'FormManagement'
    });

    $locationProvider.html5Mode({
        enabled: true
    });

}).run(['$rootScope', function ($rootScope) {

    var projectPath = $("base").first().attr("href");      // use this path in development mode

    // var projectPath = "/Client/CMS/";                    // use this path in production mode


    $rootScope.projectPath = projectPath;



    
   
}]);



angularApp.directive('numericOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {

            modelCtrl.$parsers.push(function (inputValue) {
                var transformedInput = inputValue ? inputValue.replace(/[^0-9]/g, '') : null;

                if (transformedInput != inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
        }
    };
});


angularApp.directive('yourDirective', function () {
    return {
        link: function (scope, element, attrs) {
            debugger;
            element.multiselect({
                buttonClass: 'btn',
                buttonWidth: 'auto',
                buttonContainer: '<div class="btn-group" />',
                maxHeight: false,
                buttonText: function (options) {
                    if (options.length == 0) {
                        return 'None selected <b class="caret"></b>';
                    }
                    else if (options.length > 3) {
                        return options.length + ' selected  <b class="caret"></b>';
                    }
                    else {
                        var selected = '';
                        options.each(function () {
                            selected += $(this).text() + ', ';
                        });
                        return selected.substr(0, selected.length - 2) + ' <b class="caret"></b>';
                    }
                }
            });

            // Watch for any changes to the length of our select element
            scope.$watch(function () {
                return element[0].length;
            }, function () {
                element.multiselect('rebuild');
            });

            // Watch for any changes from outside the directive and refresh
            scope.$watch(attrs.ngModel, function () {
                element.multiselect('refresh');
            });

        }

    };
});