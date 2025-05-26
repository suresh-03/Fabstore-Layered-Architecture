function toggleReviewModel() {
    $("#add-rating-container").toggleClass("active");
    $("#rating-overlay").toggleClass("active");
}

function reloadCurrentPage() {
    window.location.reload();
}

function closeReviewModelOnOutsideClick(event) {
    const $ratingContainer = $("#add-rating-container");
    const $toggleButton = $("#rate-product");

    if ($ratingContainer.length > 0 && $toggleButton.length > 0) {
        const isClickInsideReviewModel = $ratingContainer[0].contains(event.target);
        const isClickOnToggleButton = $toggleButton[0].contains(event.target);

        if (!isClickInsideReviewModel && !isClickOnToggleButton) {
            $ratingContainer.removeClass("active");
            removeRatingOverlay();
        }
    }
}

function removeRatingOverlay() {
    $("#rating-overlay").removeClass("active");
}

function initializeReviewModel() {
    // Toggle review model on menu button click
    $("#rate-product").on("click", function (e) {
        e.stopPropagation(); // Prevent event from bubbling to document
        toggleReviewModel();
    });

    // Close sidebar when clicking outside
    $(document).on("click", function (e) {
        closeReviewModelOnOutsideClick(e);
    });

    // Close sidebar on overlay click
    $("#rating-overlay").on("click", function () {
        $("#rate-product").removeClass("active");
        removeRatingOverlay();
        reloadCurrentPage();
    });

    // Close sidebar on close button click
    $("#close-review-model").on("click", function () {
        $("#add-rating-container").removeClass("active");
        removeRatingOverlay();
        reloadCurrentPage();
    });
}

initializeReviewModel();

// -------------------------------------- RATING FORM SUBMISSION --------------------------------------//

function handleStarSelection(containerSelector) {
    const $container = $(containerSelector);

    $container.find(".rating-star").on("click", function () {
        const starId = $(this).attr("id");
        const selectedRating = parseInt(starId.split("-")[1]);

        // Store selected rating in data attribute on the container
        $container.data("selected-rating", selectedRating);

        // Fill selected stars
        $container.find(".rating-star").each(function (index) {
            if (index < selectedRating) {
                $(this).removeClass("not-filled-star").addClass("filled-star");
            } else {
                $(this).removeClass("filled-star").addClass("not-filled-star");
            }
        });

        const descriptions = {
            1: { text: "Very Bad", color: "tomato" },
            2: { text: "Bad", color: "darkorange" },
            3: { text: "Good", color: "gold" },
            4: { text: "Very Good", color: "lightgreen" },
            5: { text: "Excellent", color: "green" }
        };

        const description = descriptions[selectedRating];
        $container.find(".rating-description")
            .text(description.text)
            .css("color", description.color);
    });
}


function handleAddReview(api, productId, containerSelector) {
    const $container = $(containerSelector);

    $container.find("#rating-submit-btn").on("click", function () {
        const selectedRating = $container.data("selected-rating");

        if (!selectedRating) {
            alert("Please select a rating.");
            return;
        }

        $.ajax({
            url: api,
            method: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                productId: productId,
                rating: selectedRating
            }),
            success:async function (response) {
                if (response.success) {
                    $container.find(".rating-feedback").css("display", "block");
                    await sleep(2000);
                    $container.find(".rating-feedback").css("display", "none");
                } else {
                    console.log(response);
                    alert(response.message);
                }
                resetRating(containerSelector);
            },
            error: function (xhr) {
                if (xhr.status === 401) {
                    window.location.href = "/auth/signin";
                } else {
                    alert("Error submitting rating.");
                }
                resetRating(containerSelector);
            }
        });
    });
}

function resetRating(containerSelector) {
    const $container = $(containerSelector);

    // Clear the stored rating
    $container.removeData("selected-rating");

    // Reset stars to unfilled
    $container.find(".rating-star").removeClass("filled-star").addClass("not-filled-star");

    // Reset description to default
    $container.find(".rating-description").text("Select the Rating").css("color", "");
}

function sleep(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}


