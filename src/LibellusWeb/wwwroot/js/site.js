// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function makeid(length) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for ( var i = 0; i < length; i++ ) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

function unselect(inputItemName) {
    var items = document.getElementsByName(inputItemName);
    for (const element of items) {
        element.checked = false;
    }
}

function resetThing(inputItemId) {
    var item = document.getElementById(inputItemId);
    item.value = null;
}

function removeFromDatalist(datalistId, value) {
    if (datalistId) {
        var datalist = document.getElementById(datalistId);
        var found = false;
        var index = 0;
        while (!found && index < datalist.options.length) {
            if (value === datalist.options[index].value) {
                found = true;
            }
            else {
                index++;
            }
        }

        if (found) {
            datalist.options[index].remove();
        }
    }
}

function addNewSimpleValue(inputId, inputType, datalistId, listId, listItemName) {
    var textInput = document.getElementById(inputId);
    var ul = document.getElementById(listId);

    if (textInput.value && textInput.value !== '') {
        var li = document.createElement('li');
        li.className = 'list-group-item';

        var tempId = makeid(7);

        var input = document.createElement('input');
        input.type = inputType;
        input.className = 'form-check-input ms-1 me-1';
        input.checked = true;
        input.value = textInput.value;
        input.name = listItemName;
        input.id = tempId;

        var label = document.createElement('label');
        label.htmlFor = tempId;
        label.className = 'form-check-label ms-1 me-1';
        label.appendChild(document.createTextNode(textInput.value));

        li.appendChild(input);
        li.appendChild(label);
        
        ul.appendChild(li);

        removeFromDatalist(datalistId, textInput.value)

        textInput.value = "";
    }
}

function addTrueFalseValue(inputId, inputType, datalistId, listId, listItemName, value, valueTruePostfix, valueFalsePostfix) {
    var textInput = document.getElementById(inputId);
    var ul = document.getElementById(listId);

    if (textInput.value && textInput.value !== '' && (value === 'true' || value === 'false')) {
        var li = document.createElement('li');
        li.className = 'list-group-item';

        var tempId = makeid(7);

        var input = document.createElement('input');
        input.type = inputType;
        input.className = 'form-check-input ms-1 me-1';
        input.checked = true;
        input.name = listItemName;
        input.id = tempId;

        if (value === 'true') {
            input.value = 'T-' + textInput.value;
        }
        else {
            input.value = 'F-' + textInput.value;
        }

        var label = document.createElement('label');
        label.htmlFor = tempId;
        label.className = 'form-check-label ms-1 me-1';
        label.appendChild(document.createTextNode(textInput.value));

        li.appendChild(input);
        li.appendChild(label);

        if (value === 'true' && valueTruePostfix) {
            var italic1 = document.createElement('i');
            italic1.className = 'ms-1';
            italic1.appendChild(document.createTextNode(valueTruePostfix));
            li.appendChild(italic1);
        }

        if (value === 'false' && valueFalsePostfix) {
            var italic2 = document.createElement('i');
            italic2.className = 'ms-1';
            italic2.appendChild(document.createTextNode(valueFalsePostfix))
            li.appendChild(italic2);
        }
        
        ul.appendChild(li);

        removeFromDatalist(datalistId, textInput.value)

        textInput.value = ""
    }
}