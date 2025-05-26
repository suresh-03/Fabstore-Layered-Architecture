function handleAddToWishlist(api,variantId) {
    $.ajax({
        url: api,
        type: "GET",
        data: { variantId },
        success: function (response) {
            $("#favorite-icon").addClass("favorite-icon");
        },
        error: function (xhr, error) {
            if (xhr.status == 401) {
                window.location.href = "/auth/signin";
            }
            console.error(error);
        }
    });
}

function handleWishlistAction(variantId) {
    
    $("#favorite-icon").click(function () {
        ItemExistsInWishlist("/api/wishlist/exists", variantId, function (exists) {
            if (exists) {
                handleRemoveFromWishlist("/api/wishlist/remove", variantId);
            }
            else {
                handleAddToWishlist("/api/wishlist/add", variantId);
            }
        });
    });
}

function handleRemoveFromWishlist(api,variantId) {
    $.ajax({
        url: api,
        type: "GET",
        data: { variantId },
        success: function (response) {
            $("#favorite-icon").removeClass("favorite-icon");
        },
        error: function (xhr, error) {
            if (xhr.status == 401) {
                window.location.href = "/auth/signin";
            }
            console.error(error);
        }
    });
}

function ItemExistsInWishlist(api, variantId, callback) {
    $.ajax({
        url: api,
        type: "GET",
        data: { variantId },
        success: function (response) {
            callback(response.data === true);
        },
        error: function (xhr, error) {
            if (xhr.status == 401) {
                window.location.href = "/auth/signin";
            } else {
                console.error(error);
                callback(false);
            }
        }
    });
}

function handleItemExistsInWishlist(api, variantId) {
    ItemExistsInWishlist(api, variantId, function (exists) {
        if (exists) {
            $("#favorite-icon").addClass("favorite-icon");
        } else {
            $("#favorite-icon").removeClass("favorite-icon");
        }
    });
}

function removeWishlistItem(variantId) {
        $.ajax({
            url: "/api/wishlist/remove",
            type: "GET",
            data: { variantId },
            success: function (response) {
                if (response.success) {
                    window.location.reload();
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr) {
                if (xhr.status == 401) {
                    window.location.href = "/auth/signin";
                }
                console.log(xhr.responseText);
            }
        });
    
}
