(function () {
    "use strict";

    // Защита от повторного подключения.
    if (window.NomenclaturaClassifications) {
        return;
    }

    const rootSelector = "[data-nomenclatura-classifications]";

    const goodsClassId = "1";
    const finishedProductSkuClassId = "1";

    const previousValues = new WeakMap();

    function getFields(root) {
        return {
            classSelect:
                root.querySelector('[name="SelectedClassId"]'),

            skuSelect:
                root.querySelector('[name="SelectedSkuClassId"]'),

            productTypeSelect:
                root.querySelector('[name="SelectedProductTypeId"]'),

            brandSelect:
                root.querySelector('[name="SelectedBrandId"]')
        };
    }

    function hasAllFields(fields) {
        return fields.classSelect &&
            fields.skuSelect &&
            fields.productTypeSelect &&
            fields.brandSelect;
    }

    function setAvailability(root, select, containerName, available) {
        if (!available) {
            select.value = "";
        }

        select.disabled = !available;

        // В обычных формах скрываем весь блок:
        // список, подпись, ошибки и ссылку на справочник.
        // Если контейнера нет, например в строке таблицы импорта,
        // ограничиваемся блокировкой самого списка.
        const container = root.querySelector(
            `[data-classification-field="${containerName}"]`
        );

        if (container) {
            container.hidden = !available;
        }
    }

    function refreshRoot(root) {
        const fields = getFields(root);

        if (!hasAllFields(fields)) {
            return;
        }

        const isGoods =
            fields.classSelect.value === goodsClassId;

        setAvailability(
            root,
            fields.skuSelect,
            "sku",
            isGoods
        );

        const isFinishedProduct =
            isGoods &&
            fields.skuSelect.value === finishedProductSkuClassId;

        setAvailability(
            root,
            fields.productTypeSelect,
            "product-type",
            isFinishedProduct
        );

        setAvailability(
            root,
            fields.brandSelect,
            "brand",
            isFinishedProduct
        );

        previousValues.set(root, {
            classValue: fields.classSelect.value,
            skuValue: fields.skuSelect.value
        });
    }

    function refresh(scope) {
        const target = scope || document;

        if (target.matches && target.matches(rootSelector)) {
            refreshRoot(target);
        }

        target.querySelectorAll(rootSelector).forEach(refreshRoot);
    }

    document.addEventListener("change", function (event) {
        const changedField = event.target;

        if (!(changedField instanceof HTMLSelectElement)) {
            return;
        }

        if (changedField.name !== "SelectedClassId" &&
            changedField.name !== "SelectedSkuClassId") {
            return;
        }

        const root = changedField.closest(rootSelector);

        if (!root) {
            return;
        }

        const fields = getFields(root);

        if (!hasAllFields(fields)) {
            return;
        }

        const isGoods =
            fields.classSelect.value === goodsClassId;

        const isFinishedProduct =
            isGoods &&
            fields.skuSelect.value === finishedProductSkuClassId;

        const willClearSku =
            !isGoods &&
            fields.skuSelect.value !== "";

        const willClearProductType =
            !isFinishedProduct &&
            fields.productTypeSelect.value !== "";

        const willClearBrand =
            !isFinishedProduct &&
            fields.brandSelect.value !== "";

        if (willClearSku || willClearProductType || willClearBrand) {
            const confirmed = window.confirm(
                "При изменении классификации будут очищены " +
                "неприменимые зависимые значения. Продолжить?"
            );

            if (!confirmed) {
                const previous = previousValues.get(root);

                if (previous) {
                    fields.classSelect.value = previous.classValue;
                    fields.skuSelect.value = previous.skuValue;
                }

                return;
            }
        }

        refreshRoot(root);
    });

    window.NomenclaturaClassifications = {
        refresh: refresh
    };

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", function () {
            refresh(document);
        });
    } else {
        refresh(document);
    }
})();