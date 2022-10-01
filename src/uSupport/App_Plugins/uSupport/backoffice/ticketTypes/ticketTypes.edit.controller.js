angular.module("umbraco").controller("uSupport.ticketType.edit.controller", function (
    $scope,
    appState,
    $location,
    formHelper,
    $routeParams,
    notificationsService,
    uSupportHelperServices,
    uSupportTicketTypeResources) {

    'use strict';

    var vm = this;

    vm.loading = true;
    vm.create = $routeParams.create;

    vm.page = {
        icon: "icon-document"
    };

    uSupportTicketTypeResources.getAllDataTypes().then(function (dataTypes) {
        vm.dataTypes = dataTypes;
        vm.properties = [
            {
                alias: "propertyName",
                label: "Property name",
                view: "textbox"
            },
            {
                alias: "propertyDescription",
                label: "Property description",
                view: "textbox"
            },
            {
                alias: "property",
                label: "Property",
                view: "dropdownFlexible",
                config: {
                    items: dataTypes
                }
            }
        ];
    });

    if (!vm.create) {
        uSupportTicketTypeResources.getTicketType($routeParams.id).then(function (ticketType) {

            vm.ticketType = ticketType;

            uSupportHelperServices.getChildActions("ticketTypes", "-1", ticketType.Id).then(function (currentNode) {
                vm.page = {
                    title: ticketType.Name,
                    description: ticketType.Description,
                    icon: ticketType.Icon + " " + ticketType.Color,
                    alias: ticketType.Alias,
                    menu: {
                        currentNode: currentNode,
                        currentSection: appState.getSectionState("currentSection")
                    }
                };

                if (ticketType.PropertyId !== 0) {
                    uSupportTicketTypeResources.getDataTypeFromId(ticketType.PropertyId).then(function (type) {
                        vm.properties[0].value = ticketType.PropertyName;
                        vm.properties[1].value = ticketType.PropertyDescription;
                        vm.properties[2].value = type.Name;

                        vm.loading = false;
                    });
                } else {
                    vm.loading = false;
                }
            });
        });
    } else {
        vm.loading = false;
    }

    vm.save = function () {
        vm.buttonState = "busy";

        if (formHelper.submitForm({ scope: $scope })) {

            var iconArr = vm.page.icon.split(" ");

            vm.ticketType.Name = vm.page.title;
            vm.ticketType.Description = vm.page.description;
            vm.ticketType.Alias = vm.page.alias;
            vm.ticketType.Icon = iconArr[0];
            vm.ticketType.Color = iconArr[1];
            vm.ticketType.PropertyName = vm.properties[0].value;
            vm.ticketType.PropertyDescription = vm.properties[0].value;

            if (vm.properties[2].singleDropdownValue !== '') {
                uSupportTicketTypeResources.getDataTypeFromName(vm.properties[2].singleDropdownValue).then(function (dataType) {
                    uSupportTicketTypeResources.getDataTypeViewFromEditorAlias(dataType.EditorAlias).then(function (dataTypeView) {
                        vm.ticketType.propertyId = dataType.Id;
                        vm.ticketType.propertyView = dataTypeView;

                        vm.preformSave();
                    });
                });
            } else {
                vm.preformSave();
            }
        }
    };

    vm.preformSave = function () {
        if (!vm.create) {
            uSupportTicketTypeResources.updateTicketType(vm.ticketType).then(function () {
                vm.buttonState = "success";
                $scope.formName.$dirty = false;
                notificationsService.success("uSupport", "Ticket type was successfully updated!");
                $location.url("/uSupport/ticketTypes/overview");
            }, function (err) {
                if (err.data && (err.data.message || err.data.Detail)) {
                    notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                }
                vm.buttonState = "error";
            });
        } else {
            uSupportTicketTypeResources.createTicketType(vm.ticketType).then(function (ticketType) {
                vm.buttonState = "success";
                $scope.formName.$dirty = false;
                notificationsService.success("uSupport", "A new ticket type was created successfully!");
                vm.close();
            }, function (err) {
                if (err.data && (err.data.message || err.data.Detail)) {
                    notificationsService.error("uSupport", err.data.message ?? err.data.Detail);
                }
                vm.buttonState = "error";
            });
        }
    };

    vm.close = function () {
        $location.url("/uSupport/ticketTypes/overview");
    };
});