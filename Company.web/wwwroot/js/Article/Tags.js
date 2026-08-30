/**
 * Tags helper for the article create/edit form.
 *
 * Initialises the Tagify control on #tagInput. On form submit it copies the
 * current tags into hidden TagNames fields so they are bound to the DTO.
 *
 * On the edit screen the page sets window.articleInitialTags (an array of tag
 * names) and this script pre-loads them into the control. On create the
 * variable is undefined, so the control starts empty.
 */
(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {

        var input = document.getElementById("tagInput");

        if (!input) {
            return;
        }

        var tagify = new Tagify(input);

        var form = document.getElementById("createArticleForm");
        var tagValues = document.getElementById("tagValues");

        // Pre-populate existing tags on the edit screen.
        if (window.articleInitialTags && window.articleInitialTags.length) {
            tagify.addTags(window.articleInitialTags);
        }

        if (form) {
            form.addEventListener("submit", function () {

                if (tagValues) {
                    tagValues.innerHTML = "";
                }

                tagify.value.forEach(function (tag) {

                    var hiddenInput = document.createElement("input");
                    hiddenInput.type = "hidden";
                    hiddenInput.name = "TagNames";
                    hiddenInput.value = tag.value;

                    if (tagValues) {
                        tagValues.appendChild(hiddenInput);
                    }
                });

            });
        }

    });
})();
