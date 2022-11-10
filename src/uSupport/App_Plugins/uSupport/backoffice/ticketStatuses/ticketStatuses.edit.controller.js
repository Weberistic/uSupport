﻿angular.module("umbraco").controller("uSupport.ticketStatus.edit.controller", function (
    $scope,
    appState,
    formHelper,
    $location,
    $routeParams,
    uSupportConfig,
    notificationsService,
    uSupportHelperServices,
    uSupportTicketStatusResources) {

    'use strict';

    var vm = this;

    vm.page = {};
    vm.loading = true;
    vm.create = $routeParams.create;
    vm.properties = uSupportConfig.ticketStatusProperties;

    vm.preview = {
        property: vm.properties[0],
        page: vm.page
    }

    if (!vm.create) {
        uSupportTicketStatusResources.getTicketStatus($routeParams.id).then(function (ticketStatus) {
            vm.ticketStatus = ticketStatus;

            uSupportHelperServices.getChildActions("ticketStatuses", "-1", ticketStatus.Id).then(function (currentNode) {
                vm.page = {
                    title: ticketStatus.Name,
                    alias: ticketStatus.Alias,
                    menu: {
                        currentNode: currentNode,
                        currentSection: appState.getSectionState("currentSection")
                    }
                };

                vm.preview.page = vm.page;
      
                vm.properties.forEach(function (property) {
                    property.value = vm.ticketStatus[property.alias];
                });

                vm.loading = false;
            });
        });
    } else {
        vm.loading = false;
    }

    $scope.$watch("vm.preview", function (newValue, oldValue) {
        vm.selectedColor = newValue.property.singleDropdownValue;
        vm.selectedName = newValue.page.title;
    }, true);

    vm.save = function () {
        vm.buttonState = "busy";

        if (formHelper.submitForm({ scope: $scope })) {

            vm.ticketStatus = {
                Name: vm.page.title,
                Alias: vm.page.alias
            };

            vm.properties.forEach(function (property) {
                if (property.alias === "Color") {
                    vm.ticketStatus[property.alias] = property.singleDropdownValue
                } else {
                    vm.ticketStatus[property.alias] = parseInt(property.value)
                }
            });

            if (!vm.create) {
                uSupportTicketStatusResources.updateTicketStatus(vm.ticketStatus).then(function () {
                    vm.buttonState = "success";
                    $scope.formName.$dirty = false;
                    notificationsService.success("uSupport", "Ticket status was updated successfully!");
                    vm.close();
                }, function (err) {
                    if (err.data && (err.data.message || err.data.Detail)) {
                        notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                    }
                    vm.buttonState = "error";
                });
            } else {
                uSupportTicketStatusResources.createTicketStatus(vm.ticketStatus).then(function (status) {
                    vm.buttonState = "success";
                    $scope.formName.$dirty = false;
                    notificationsService.success("uSupport", "A new ticket status was created successfully!");
                    vm.close();
                }, function (err) {
                    if (err.data && (err.data.message || err.data.Detail)) {
                        notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                    }
                    vm.buttonState = "error";
                });
            }
        } else {
            vm.buttonState = "error";
            notificationsService.error("uSupport", "Failed to create ticket status");
        }
    };

    vm.close = function () {
        $location.url("/uSupport/ticketStatuses/overview");
    };
});