//loadComments();

function loadComments() {
    var loadCommentLink = document.getElementById('loadComments');

    loadCommentLink.addEventListener('click', function () {
        window.commento.main();
        loadCommentLink.parentNode.removeChild(loadCommentLink);
    });
}
