ko.validation.rules['url'] = {
    validator: function (val, required) {
        if (!val) {
            return !required
        }
        val = val.replace(/^\s+|\s+$/, ''); //Strip whitespace
        //Regex by Diego Perini from: http://mathiasbynens.be/demo/url-regex
        return val.match(/[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()@:%_\+.~#?&//=]*)?/gi);
    },
    message: 'A valid URL should start with http:// or https://'
};