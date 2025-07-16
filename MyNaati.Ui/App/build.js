var requirejs = require('requirejs');
var filesystem = require('fs');

// Determine if a given file should be added to the results list
var _getFile = function (fileSearches, file) {
    var result = null;
    var addedFile = false;

    fileSearches.forEach(function (fileSearch) {
        if (!addedFile) {
            var includeFile = false;

            fileSearch.include.forEach(function (include) {
                if (!includeFile && include.test(file)) {
                    includeFile = true;
                }
            });

            if (includeFile && fileSearch.exclude) {
                fileSearch.exclude.forEach(function (exclude) {
                    if (includeFile && exclude.test(file)) {
                        includeFile = false;
                    }
                });
            }

            if (includeFile) {
                result = fileSearch.result(file);
                addedFile = true;
            }
        }
    });

    return result;
}

// Recursive function to find specified files inside the given directory
var _getFiles = function (directory, excludedDirectories, fileSearches) {
    var results = [];

    filesystem.readdirSync(directory).forEach(function (file) {
        file = directory + '/' + file;
        var stat = filesystem.statSync(file);

        if (stat && stat.isDirectory()) {
            var exclude = false;

            if (excludedDirectories) {
                excludedDirectories.forEach(function (excludedDirectory) {
                    if (!exclude && excludedDirectory.test(file)) {
                        exclude = true;
                    }
                });
            }

            if (!exclude) {
                results = results.concat(_getFiles(file, excludedDirectories, fileSearches));
            }
        } else {
            var result = _getFile(fileSearches, file);

            if (result) {
                results.push(result);
            }
        }
    });

    return results;
};

var _getFileSubstringStartIndex = function (file) {
    // If file name starts with './' remove it
    if (/^.\//.test(file)) {
        return 2;
    }

    return 0;
}

var fileSearches = [{
    include: [/\.js$/],
    exclude: [
        /build/, /main.dist/, // Don't include itself
        // Explicitly added below, so exclude it from our search
        /constant/,
        /requiredModules/,
        /main/
    ],
    result: function (file) {
        return file.substring(_getFileSubstringStartIndex(file), file.length - 3);
    }
}, {
    include: [/\.html$/],
    result: function (file) {
        return 'text!' + file.substring(_getFileSubstringStartIndex(file), file.length);
    }
}];

// List of directories that we want included in a specific order
var directorySearches = [
    { directory: 'customBindings' },
    { directory: 'validationRules' },
    { directory: 'components' }
];

// List of directories we want excluded from the root directory
var excludeFromRoot = [/node_modules/];

// Add to that exclude list, any directories we've already explicitly added to the directory search list
directorySearches.forEach(function (directorySearch) {
    excludeFromRoot.push(new RegExp(directorySearch.directory));
});

// Add the root directory, excluding any explicity excluded directories,
// and any directories already added to the search list
directorySearches.push({
    directory: '.',
    exclude: excludeFromRoot
});

// Add any files we want to be explicity added in a specific order
// Remember to add it the respective file search's exclude list above
var files = [
    'services/constant'
];

// Perform our search
directorySearches.forEach(function (directorySearch) {
    files = files.concat(_getFiles(directorySearch.directory, directorySearch.exclude, fileSearches));
});

//files.push('requiredModules');
files.push('main');

var required = [];

files.forEach(function (file) {
    if (/customBindings/.test(file)) {
        required.push(file);
    }
});

required = required.concat([
    'main' // Must be added last
]);

// Put together our final configuration
var baseConfig = {
    paths: {
        durandal: '../Scripts/durandal',
        plugins: '../Scripts/durandal/plugins',
        text: '../Scripts/text'
    },

    shim: {},

    // (un)comment the next line to disable/enable minification
    //optimize: "none",

    baseUrl: "",
    name: "main",
    out: "main.dist.js",
    removeCombined: true,
    include: files,
    // Certain modules, for example custom bindings, are never explicity required using require.js
    // Which means they are never loaded, and trying to use the them in a html template won't work
    // If a module fits that description try adding it here to explicity require/include it
    insertRequire: required
};

// Note: if this is ran using the OptimizeRequire.ps1 script,
// logging to the console will be considered an error.
// i.e. the script will report an error when there may not be one,
// and main.dist.js was most likely successfully built anyway.

//console.log(files);

// Do some magic!
requirejs.optimize(baseConfig);
