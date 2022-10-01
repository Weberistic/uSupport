(function (uSupportHelperResources) {
    'use strict';
    angular.module("umbraco").constant("uSupportConfig", {
        "baseApiUrl": "/umbraco/backoffice/api/",
        "basePathAppPlugins": "/App_Plugins/uSupport/",
        "ticketStatusProperties": [
            {
                alias: "Color",
                label: "Color",
                view: "dropdownFlexible",
                value: "primary",
                config: {
                    items: [
                        "primary",
                        "secondary",
                        "success",
                        "warning",
                        "danger",
                        "gray"
                    ],
                },
                validation: {
                    mandatory: true,
                    //  pattern: ""
                }
            },
            {
                alias: "Active",
                label: "Active",
                description: "If tickets with this status should count as active",
                view: "boolean",
            },
            {
                alias: "default",
                label: "Default",
                description: "If all new tickets should have this status as default status. (Only 1 can be default)",
                view: "boolean",
            }
        ]
    });
})();