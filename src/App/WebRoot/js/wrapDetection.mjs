const flexBoxQuery = ".flexWrapDetect";
const boxWrappedClass = "flex_box-wrapped";

// Rounded for inline-flex sub-pixel discrepencies:
const getTop = (item) => Math.round(item.getBoundingClientRect().top);

const markFlexboxAndItemsWrapState = (flexBox) => {
  // Acts as a throttle,
  // Prevents hitting ResizeObserver loop limit,
  // Optimal timing for visual change:
  requestAnimationFrame((_) => {
    const flexItems = flexBox.children;

    // Needs to be in a row for the calculations to work
    flexBox.setAttribute("style", "flex-direction: row");

    const firstItemTop = getTop(flexItems[0]);
    const lastItemTop = getTop(flexItems[flexItems.length - 1]);

    // Add / remove wrapped class to each wrapped item
    for (const flexItem of flexItems) {
      if (firstItemTop < getTop(flexItem)) {
        flexItem.classList.remove("border-2");
        flexItem.classList.add("border-x-2");
        flexItem.classList.add("border-b-2");
        flexItem.classList.add("wrappedItem");
      } else {
        flexItem.classList.add("border-2");
        flexItem.classList.remove("border-x-2");
        flexItem.classList.remove("border-b-2");
        flexItem.classList.remove("wrappedItem");
      }
    }

    const firstRowItems = flexBox.querySelectorAll(":not(.wrappedItem)");
    firstRowItems[firstRowItems.length - 1].classList.remove("border-r-0");
    firstRowItems[firstRowItems.length - 1].classList.add("border-r-2");

    const wrappedRowItems = flexBox.querySelectorAll(".wrappedItem");
    if (wrappedRowItems.length) {
      wrappedRowItems[wrappedRowItems.length - 1].classList.remove(
        "border-r-0"
      );
      wrappedRowItems[wrappedRowItems.length - 1].classList.add("border-r-2");
    }

    // Remove flex-direction:row used for calculations
    flexBox.removeAttribute("style");

    // Add / remove wrapped class to the flex container
    if (firstItemTop >= lastItemTop) {
      flexBox.classList.remove(boxWrappedClass);
    } else {
      flexBox.classList.add(boxWrappedClass);
    }
  });
};

const flexBoxes = document.querySelectorAll(flexBoxQuery);
for (const flexBox of flexBoxes) {
  markFlexboxAndItemsWrapState(flexBox);

  new ResizeObserver((entries) =>
    entries.forEach((entry) => markFlexboxAndItemsWrapState(entry.target))
  ).observe(flexBox);
}
