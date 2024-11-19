document.addEventListener("DOMContentLoaded", function () {
    let eventClickId = "";
    let isExecutedReplaceBr = false;
    let isExecutedSetAuth = false;

    const replaceBr = function (description) {
        const formattedHtml = description.innerHTML
            .replace(/\n\s*\n/g, "<br />") // Thay thế hai dòng xuống liên tiếp thành một <br />
            .replace(/\n/g, "<br />") // Thay thế một dòng xuống
            .replace(/(<br \/>)+/g, "<br />") // Loại bỏ các <br /> thừa
            .replace(/<br \/>Created by/g, "- Created by") // Xoá <br/> trước "Created by"
            .replace(/<br \/>Modified by/g, "- Modified by"); // Xoá <br/> trước "Updated by"

        description.innerHTML = formattedHtml;
        isExecutedReplaceBr = true;
    };

    const updateDescriptions = function (descriptionElements) {
        descriptionElements.forEach(replaceBr);
    };

    const setAuth = function () {
        const swaggerToken = localStorage.getItem("swagger_access_token");
        const unlockButton = document.querySelector(
            "button.authorize.unlocked"
        );

        if (swaggerToken && unlockButton) {
            unlockButton.click();

            const bearerValueElement =
                document.getElementById("auth-bearer-value");
            if (bearerValueElement) {
                const nativeInputValueSetter = Object.getOwnPropertyDescriptor(
                    window.HTMLInputElement.prototype,
                    "value"
                ).set;

                nativeInputValueSetter.call(bearerValueElement, swaggerToken);
                bearerValueElement.dispatchEvent(
                    new Event("change", { bubbles: true })
                );

                document
                    .querySelector(".auth-container button.authorize")
                    .click();
                document
                    .querySelector(".auth-container button.btn-done")
                    .click();

                isExecutedSetAuth = true;
            }
        } else if (!swaggerToken) {
            isExecutedSetAuth = true;
        }
    };

    const handleAuthClick = function () {
        const accessToken = document.querySelector("#auth-bearer-value").value;
        localStorage.setItem("swagger_access_token", accessToken);
    };

    const saveAccessToken = function () {
        const mutationObserver = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.addedNodes.length) {
                    const authButton = document.querySelector(
                        ".auth-container button.authorize"
                    );

                    if (authButton) {
                        authButton.addEventListener("click", handleAuthClick, {
                            once: true,
                        });
                    }

                    const logoutButton = document.querySelector(
                        `.button.auth[aria-label="Remove authorization"]`
                    );

                    if (logoutButton) {
                        logoutButton.addEventListener("click", function () {
                            localStorage.removeItem("swagger_access_token");
                        });
                    }
                }
            });
        });

        mutationObserver.observe(document.body, {
            childList: true,
            subtree: true,
        });
    };

    let replaceBrId = setInterval(function () {
        // Cập nhật mô tả khi nhấn vào các thẻ
        document.querySelectorAll(".opblock-tag").forEach(function (tag) {
            tag.addEventListener("click", function () {
                clearTimeout(eventClickId);
                eventClickId = setTimeout(function () {
                    // Phần tử tiếp theo của tag
                    const nextElement = tag.nextElementSibling;
                    nextElement &&
                        updateDescriptions(
                            nextElement.querySelectorAll(
                                ".opblock-summary-description"
                            )
                        );
                }, 500);
            });
        });

        updateDescriptions(
            document.querySelectorAll(".opblock-summary-description")
        );

        if (isExecutedReplaceBr) {
            clearInterval(replaceBrId);
        }
    }, 100);

    let setAuthId = setInterval(function () {
        setAuth();
        if (isExecutedSetAuth) {
            clearInterval(setAuthId);
        }
    }, 100);

    saveAccessToken();
});
