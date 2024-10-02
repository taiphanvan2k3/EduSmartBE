document.addEventListener("DOMContentLoaded", function () {
    let eventClickId = "";

    const replaceBr = function (description) {
        const formattedHtml = description.innerHTML
            .replace(/\n\s*\n/g, "<br />") // Thay thế hai dòng xuống liên tiếp thành một <br />
            .replace(/\n/g, "<br />") // Thay thế một dòng xuống
            .replace(/(<br \/>)+/g, "<br />") // Loại bỏ các <br /> thừa
            .replace(/<br \/>Created by/g, "- Created by"); // Xoá <br/> trước "Created by"

        description.innerHTML = formattedHtml;
    };

    const updateDescriptions = function () {
        const descriptions = document.querySelectorAll(
            ".opblock-summary-description"
        );

        descriptions.forEach(replaceBr);
    };

    setTimeout(function () {
        // Cập nhật mô tả khi nhấn vào các thẻ
        document.querySelectorAll(".opblock-tag").forEach(function (tag) {
            tag.addEventListener("click", function () {
                clearTimeout(eventClickId);
                eventClickId = setTimeout(updateDescriptions, 200);
            });
        });

        updateDescriptions();
    }, 200);
});
