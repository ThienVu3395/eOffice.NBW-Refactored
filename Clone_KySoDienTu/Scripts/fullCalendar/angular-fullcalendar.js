(function () { 
'use strict';
angular.module('angular-fullcalendar',[])
    .value('CALENDAR_DEFAULTS', {
        locale: 'vi'
    })
    .directive('fc', ['CALENDAR_DEFAULTS', '$rootScope', fcDirectiveFn])
        function fcDirectiveFn(CALENDAR_DEFAULTS, $rootScope) {
            return {
                restrict: 'A',
                scope: {
                    objectToInject: '=', eventSource: '=ngModel', options: '=fcOptions',
                    calTitle: '='
                },
                link: function (scope, elm, attributes) {
                    var calendar;
                    init();
                    scope.$watch('eventSource', watchDirective, true);
                    scope.$watch('options', watchDirective, true);
                    scope.$watch('calTitle', function () {
                    });
                    scope.$on('$destroy', function () { destroy(); });
                    scope.$watch('objectToInject', function (value) {
                        if (value) {
                            scope.Obj = value;
                            scope.Obj.today = function () {
                                calendar.fullCalendar('today');
                                $rootScope.calTitle = calendar.fullCalendar('getView').title;
                            }
                            scope.Obj.next = function () {
                                calendar.fullCalendar('next');
                                $rootScope.calTitle = calendar.fullCalendar('getView').title;
                            }
                            scope.Obj.prev = function () {
                                calendar.fullCalendar('prev');
                                $rootScope.calTitle = calendar.fullCalendar('getView').title;
                            }
                            scope.Obj.month = function () {
                                calendar.fullCalendar('changeView', 'month');
                                $rootScope.calTitle = calendar.fullCalendar('getView').title;
                            }
                            scope.Obj.week = function () {
                                calendar.fullCalendar('changeView', 'agendaWeek');
                                $rootScope.calTitle = calendar.fullCalendar('getView').title;
                            }
                            //scope.Obj.day = function () {
                            //    calendar.fullCalendar('changeView', 'timelineDay');
                            //    $rootScope.calTitle = calendar.fullCalendar('getView').title;
                            //}
                        }
                    });
                    function init() {
                        if (!calendar) {
                            calendar = $(elm).html('');
                        }
                        calendar.fullCalendar(getOptions(scope.options));
                        $rootScope.calTitle = calendar.fullCalendar('getView').title;
                    }
                    function destroy() {
                        if (calendar && calendar.fullCalendar) {
                            calendar.fullCalendar('destroy');
                        }
                    }
                    function getOptions(options) {
                        return angular.extend(CALENDAR_DEFAULTS, {
                            events: scope.eventSource
                        }, options);
                    }
                    function watchDirective(newOptions, oldOptions) {
                        if (newOptions !== oldOptions) {
                            destroy();
                            init();
                        } else if ((newOptions && angular.isUndefined(calendar))) {
                            init();
                        }
                    }
                }
            };
        }
}());