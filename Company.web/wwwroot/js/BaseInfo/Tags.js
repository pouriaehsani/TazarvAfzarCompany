/**
 * BaseInfo > Tags panel.
 *
 * Renders no tags by itself — the list is server-rendered. This script only
 * wires up the per-row delete action: it confirms with the user, POSTs to the
 * delete endpoint, and removes the row from the DOM on success.
 *
 * The server owns all business rules (existence, referential integrity).
 * The anti-forgery token is sent both as a form field and as the conventional
 * RequestVerificationToken header, so [ValidateAntiForgeryToken] always
 * accepts the request.
 */
(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {

        var tableBody = document.getElementById("tagTableBody");

        // Fail quietly if the panel is not present on this page.
        if (!tableBody) {
            return;
        }

        var errorEl = document.getElementById("tagError");
        var messageEl = document.getElementById("tagMessage");

        /**
         * Reads the anti-forgery token rendered by @Html.AntiForgeryToken().
         */
        function readToken() {
            var input = document.querySelector(
                "input[name='__RequestVerificationToken']"
            );

            return input ? input.value : "";
        }

        /**
         * POSTs a tag deletion and resolves with the parsed JSON. Throws an
         * Error with a user-friendly message on failure.
         */
        function postDelete(id) {

            var body = new FormData();
            body.append("__RequestVerificationToken", readToken());

            return fetch("/Admin/BaseInfo/DeleteTag/" + id, {
                method: "POST",
                headers: {
                    "RequestVerificationToken": readToken()
                },
                body: body
            }).then(function (response) {

                return response.json().then(function (payload) {
                    if (!response.ok) {
                        var message =
                            (payload && payload.message)
                                ? payload.message
                                : "Request failed.";

                        throw new Error(message);
                    }

                    return payload;
                });

            });
        }

        // Event delegation: a single listener handles every row's delete
        // button, including rows added later.
        tableBody.addEventListener("click", function (event) {

            var button = event.target.closest
                ? event.target.closest("button.btn-delete-tag")
                : null;

            if (!button) {
                return;
            }

            var id = button.dataset.id;

            if (!confirm("Are you sure you want to delete this tag?")) {
                return;
            }

            if (messageEl) messageEl.textContent = "";
            if (errorEl) errorEl.textContent = "";

            postDelete(id)
                .then(function (payload) {
                    if (messageEl) {
                        messageEl.textContent =
                            (payload && payload.message)
                                ? payload.message
                                : "Tag deleted.";
                    }

                    // Remove the row immediately so the list reflects the
                    // change without a full page reload.
                    var row = button.closest("tr");
                    if (row) {
                        row.remove();
                    }
                })
                .catch(function (error) {
                    if (errorEl) {
                        errorEl.textContent =
                            error.message || "Could not delete the tag.";
                    }
                });

        });

    });
})();
