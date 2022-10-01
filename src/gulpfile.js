/// <binding ProjectOpened='default' />
//
const { watch, src, dest } = require("gulp");
var config = require("./paths.json");

function copy(path, baseFolder, target) {
    console.log("copy: \x1b[36m%s\x1b[0m %s", path, target);  

    src(path, { base: baseFolder }).pipe(dest(target));
};

function watchAppPlugins(sources, folderName) {
    console.log("Watching : " + sources);

    config.sites.forEach((dest, i) => {
        console.log("Target   : " + dest);
    });

    sources.forEach(source => {
        watch(source, { ignoreInitial: false })
        .on("change", function (path, stats) {
            config.sites.forEach(dest => {
                copy(path, source, dest +  folderName)
            });
        })
        .on("add", function (path, stats) {
            config.sites.forEach(dest => {
                copy(path, source, dest +  folderName)
            });
        });
    });
};

exports.default = function () {
    watchAppPlugins(config.sources, config.folderNames);
};