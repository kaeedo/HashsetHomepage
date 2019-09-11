var MQL = 1170;

const topBar = document.getElementsByClassName('Main-headerTopBar')[0];

if (window.innerWidth > MQL) {
    var headerHeight = topBar.style.height;

    window.addEventListener('scroll', function () {
        var currentTop = window.scrollY;
        //check if user is scrolling up
        if (currentTop < this.previousTop || 0) {
        console.log('going up')

            //if scrolling up...
            if (currentTop > 0 && topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-visible');
            } else {
                topBar.classList.remove('is-visible');
                topBar.classList.remove('is-fixed');
            }
        } else {
            //if scrolling down...
            console.log('down')
            topBar.classList.remove('is-visible');
            if (currentTop > headerHeight && !topBar.classList.contains('is-fixed')) {
                topBar.classList.add('is-fixed');
            }
        }
        this.previousTop = currentTop;
    }, false);
}
