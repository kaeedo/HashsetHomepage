var MQL = 1170;

const topBar = document.getElementsByClassName('Main-headerTopBar')[0];

if (window.innerWidth > MQL) {
    var headerHeight = topBar.style.height;

    window.addEventListener('scroll', function () {
        var currentTop = window.scrollTop;
        console.log(currentTop) // this is undefined
        //check if user is scrolling up
        if (currentTop < this.previousTop || 0) {
            //if scrolling up...
            if (currentTop > 0 && topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-visible');
            } else {
                topBar.classList.remove('is-visible');
                topBar.classList.remove('is-fixed');
            }
        } else {
            //if scrolling down...
            topBar.classList.remove('is-visible');
            if (currentTop > headerHeight && !topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-fixed');
            }
        }
        this.previousTop = currentTop;
    }, false);
}
