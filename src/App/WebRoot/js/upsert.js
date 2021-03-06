(function () {
    var addTagButton = document.getElementById('Upsert-addTagButton');
    var removeTagButtons = document.getElementsByClassName('Upsert-removeTagButton');
    var ul = document.getElementById('Upsert-tagList');
    var submitButton = document.getElementById('Upsert-submit');
    var select = document.getElementById('Id');

    select.addEventListener('change', function (e, d) {
        e.preventDefault();
        var href = window.location.href.split('?')[0];

        if (e.target.value === '0') {
            window.location.href = href;
            return;
        }

        window.location.href = href + '?id=' + e.target.value;
    })

    submitButton.addEventListener('click', function (e) {
        e.preventDefault();

        var formNames = [
            'Id',
            'ArticleDate',
            'Title',
            'Description',
            'Source',
            'Tags'
        ]

        var isValid = formNames.map(function (fn) {
            var elem = document.getElementsByName(fn);
            if (!elem.length) {
                return false;
            }

            return !!elem[0].value;
        }).reduce(function (acc, cur) {
            return acc && cur;
        }, true)

        if (isValid) {
            document.getElementsByClassName('PostContents')[0].submit();
        } else {
            alert('Form not valid');
        }
    });

    addTagButton.addEventListener('click', function (e) {
        e.preventDefault();
        var div = document.createElement('div');
        div.classList = 'Upsert-tagValue';

        var li = document.createElement('li');
        li.classList = 'Upsert-tagListItem';

        var input = document.createElement('input');
        input.type = 'text';
        input.classList = 'Upsert-tagName';
        input.setAttribute('name', 'Tags');

        var button = document.createElement('input');
        button.type = 'button'
        button.classList = 'Upsert-removeTagButton';
        button.value = '-';
        button.addEventListener('click', removeTag);

        div.appendChild(input);
        div.appendChild(button);

        li.appendChild(div);
        ul.appendChild(li);
    });

    var removeTag = function (e) {
        e.preventDefault();
        var li = this.parentNode.parentNode;
        li.remove();
    }

    for (var i = 0; i < removeTagButtons.length; i++) {
        removeTagButtons[i].addEventListener('click', removeTag);
    }
})();



