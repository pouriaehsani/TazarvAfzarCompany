const input = document.getElementById('tagInput');

const tagify = new Tagify(input);

const form = document.getElementById('createArticleForm');

const tagValues = document.getElementById('tagValues');


form.addEventListener('submit', function (event) {

    tagValues.innerHTML = '';

    tagify.value.forEach(function (tag) {

        const hiddenInput = document.createElement('input');

        hiddenInput.type = 'hidden';
        hiddenInput.name = 'TagNames';
        hiddenInput.value = tag.value;

        tagValues.appendChild(hiddenInput);
    });


    console.log("Tagify:", tagify.value);

    console.log(
        "Hidden:",
        document.querySelectorAll('input[name="TagNames"]')
    );

    console.log(
        "Form:",
        new FormData(form).getAll('TagNames')
    );

});
