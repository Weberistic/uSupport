angular.module("umbraco").controller("uSupport.tickets.edit.controller", function (
    $q,
    $scope,
    appState,
    formHelper,
    userService,
    $routeParams,
    $location,
    uSupportTicketResources,
    uSupportTicketCommentResources,
    uSupportTicketStatusResources,
    uSupportTicketTypeResources,
    uSupportConfig,
    uSupportHelperServices,
    notificationsService) {

    'use strict';

    var vm = this;
    vm.loading = true;
    vm.buttonState = "init";

    uSupportTicketResources.getTicket($routeParams.id).then(function (ticket) {

        vm.ticket = ticket;

        $q.all({
            user: userService.getCurrentUser(),
            statuses: uSupportTicketStatusResources.getAllTicketStatuses(),
            types: uSupportTicketTypeResources.getAllTicketTypes(),
            getChildActions: uSupportHelperServices.getChildActions("tickets", "-1", ticket.Id)
        }).then(function (promises) {

            vm.page = {
                title: "Edit ticket: " + ticket.Title,
                description: "See ticket details and try to reslove ticket",
                menu: {
                    currentNode: promises.getChildActions,
                    currentSection: appState.getSectionState("currentSection")
                }
            };

            vm.user = promises.user;

            var statusNamesArr = promises.statuses.map(function (status) {
                return status.Name;
            });

            var typesNamesArr = promises.types.map(function (type) {
                return type.Name;
            });

            vm.properties = [
                {
                    alias: "ticketTitle",
                    label: "Ticket title",
                    view: "readonlyvalue",
                    value: vm.ticket.Title
                },
                {
                    alias: "ticketSummary",
                    label: "Ticket summary",
                    view: "readonlyvalue",
                    value: vm.ticket.Summary
                },
                {
                    alias: "type",
                    label: "Type",
                    view: "dropdownFlexible",
                    value: vm.ticket.Type.Name,
                    config: {
                        items: typesNamesArr
                    },
                    validation: {
                        mandatory: true,
                        pattern: ""
                    },
                }
            ];

            if (ticket.Type.PropertyView !== null) {
                vm.properties.push({
                    alias: ticket.Type.PropertyAlias,
                    label: vm.ticket.Type.PropertyName,
                    view: uSupportConfig.basePathAppPlugins + "components/propertyEditors/readonly/readonly.html",
                    value: vm.ticket.PropertyValue,
                    admin: true
                });
            }

            vm.properties.push({
                alias: "status",
                label: "Status",
                view: "dropdownFlexible",
                value: vm.ticket.Status.Name,
                config: {
                    items: statusNamesArr
                },
                validation: {
                    mandatory: true,
                    pattern: ""
                },
            });

            vm.loading = false;
        });
    });

    vm.save = function (sendEmail) {

        if (sendEmail) {
            vm.SaveAndSendbuttonState = "busy";
        } else {
            vm.SavebuttonState = "busy";
        }

        if (formHelper.submitForm({ scope: $scope })) {

            $q.all({
                typeId: uSupportTicketTypeResources.getTypeIdFromName(getPropertyValue("type")[0].value),
                statusId: uSupportTicketStatusResources.getStatusIdFromName(getPropertyValue("status")[0].value),
            }).then(function (promises) {

                vm.ticket.StatusId = promises.statusId;
                vm.ticket.TypeId = promises.typeId;
                vm.ticket.LastUpdatedBy = vm.user.name;

                uSupportTicketResources.updateTicket(vm.ticket, sendEmail).then(function () {
                    if (sendEmail) {
                        vm.SaveAndSendbuttonState = "success";
                    } else {
                        vm.SavebuttonState = "success";
                    }
                    notificationsService.success("uSupport", "successfully updated ticket!");
                    $scope.formName.$dirty = false;
                    vm.close();
                }, function (err) {
                    if (err.data && (err.data.message || err.data.Detail)) {
                        notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                    }
                    if (sendEmail) {
                        vm.SaveAndSendbuttonState = "error";
                    } else {
                        vm.SavebuttonState = "error";
                    }
                });
            });
        } else {
            if (sendEmail) {
                vm.SaveAndSendbuttonState = "error";
            } else {
                vm.SavebuttonState = "error";
            }
        }
    };

    vm.close = function () {
        $location.path("/uSupport/tickets/overview");
    };

    var getPropertyValue = function (alias) {
        return vm.properties.filter(function (property) {
            if (property.alias === alias) {
                return property.value;
            }
        });
    };
});