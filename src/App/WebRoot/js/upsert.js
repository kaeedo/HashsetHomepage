(function(){
    var tagLabel = document.getElementById('TagLabel');
    var ul = document.getElementById('TagList');

    tagLabel.addEventListener('click', function() {
        var li = document.createElement('li');
        var input = document.createElement('input');
        input.setAttribute('name', 'Tags');
        li.appendChild(input);
        ul.appendChild(li);
    });
})();



