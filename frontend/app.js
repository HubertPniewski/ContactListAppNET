// NOTE: For easier testing, all the code is in the one file  

// Backend URL
const API_URL = "https://localhost:7035/api";

// returns JWT token from local storage
function getToken() {
    return localStorage.getItem("jwt_token");
}

// returns user's email from local storage
function getUserEmail() {
    return localStorage.getItem("user_email");
}

// helper JWT functions
// decode JWT token
function getJwtPayload() {
    const token = getToken();
    if (!token) return null;

    try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');

        const jsonPayload = decodeURIComponent(window.atob(base64).split('').map(function (c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));

        return JSON.parse(jsonPayload);
    } catch (e) {
        console.error("Token decoding error: ", e);
        return null;
    }
}

// get logged in user's Id
function getUserId() {
    const payload = getJwtPayload();
    return payload ? payload.nameid : null;
}

// updating the look of the Navbar
function updateNavbar() {
    // get JWT token
    const token = getToken();

    // get page's variable elements
    const anonZone = document.getElementById("anon-zone");
    const userZone = document.getElementById("user-zone");
    const userEmailDisplay = document.getElementById("user-email-display");
    const addContactBtn = document.getElementById("btn-add-contact-mode");

    // change page's look based on token
    if (token) {
        // logged in 
        anonZone.classList.add("d-none"); // hide anonymous zone
        userZone.classList.remove("d-none"); // show logged in user zone
        userEmailDisplay.textContent = `Zalogowany (${getUserEmail()})`; // display logged in user
        addContactBtn.classList.remove("d-none"); // show "add new contact" button
    } else {
        // not logged in
        anonZone.classList.remove("d-none"); // show anonymous zone
        userZone.classList.add("d-none"); // hide logged in user zone
        addContactBtn.classList.add("d-none"); // hide "add new contact" button
    }
}

// Event called after the page is loaded
document.addEventListener("DOMContentLoaded", () => {
    updateNavbar();
    initStaticCategories();
    loadContacts();
    initAddContactMode();
});


// --------------------------------------------- AUTHENTICATION --------------------------------------------- 

// Registration
document.getElementById("register-form").addEventListener("submit", async (e) => {
    e.preventDefault(); // stop default page reloading

    // get credentials from the form
    const email = document.getElementById("register-email").value;
    const password = document.getElementById("register-password").value;

    // request to API
    try {
        const response = await fetch(`${API_URL}/auth/register`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        // extract data from the response 
        const data = await response.json();

        if (response.ok) {
            // registration successfull
            alert("Rejestracja udana! Możesz się zalogować na utworzone konto.");
            bootstrap.Modal.getInstance(document.getElementById("registerModal")).hide();
        } else {
            // registration failed
            alert(`Rejestracja zakończona niepowodzeniem z powodu błędu:\n${data.message || data}`)
        }
    } catch (err) {
        alert(`Błąd: ${err.message || err}`);
    }
});

// Login
document.getElementById("login-form").addEventListener("submit", async (e) => {
    e.preventDefault(); // stop default page reloading 

    // get credentials from the form
    const email = document.getElementById("login-email").value;
    const password = document.getElementById("login-password").value;

    // request to API
    try {
        const response = await fetch(`${API_URL}/auth/login`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        // extract data from the response 
        const data = await response.json();

        if (response.ok) {
            // login successfull
            // save JWT token and user email in local storage
            localStorage.setItem("jwt_token", data.token);
            localStorage.setItem("user_email", data.email);

            // update interface
            bootstrap.Modal.getInstance(document.getElementById("loginModal")).hide();
            updateNavbar();
            loadContacts();
        } else {
            // login failed
            alert(`Logowanie nieudane:\n${data.message || data}`)
        }
    } catch (err) {
        alert(`Błąd: ${err.message || err}`);
    }
});

// Logout
document.getElementById("btn-logout").addEventListener("click", async (e) => {
    // remove JWT token and user email from local storage
    localStorage.removeItem("jwt_token");
    localStorage.removeItem("user_email");

    // update UI
    updateNavbar();
    loadContacts();
    document.getElementById("details-card").classList.add("d-none");
    document.getElementById("no-selection-alert").classList.remove("d-none");

    alert("Wylogowano.");
});


// --------------------------------------------- CRUD LOGIC --------------------------------------------- 
// list of contacts
let contacts = [];

// function loading contacts and displaying contact previews list
async function loadContacts() {
    // get the container for contacts
    const listContainer = document.getElementById("contacts-list");

    // request to the API and displaying contacts
    try {
        const response = await fetch(`${API_URL}/contacts`);
        if (!response.ok) throw new Error("Failed to fetch contats."); // if fetching failed

        contacts = await response.json(); // save contats in contacts list

        // clean contact list before loading lastly fetched contacts
        listContainer.innerHTML = "";

        // if there's no contact in the list, display this information
        if (contacts.length === 0) {
            listContainer.innerHTML = '<div class="text-center p-3 text-muted">Brak kontaktów w bazie.</div>';
            return;
        }

        // render all contacts 
        contacts.forEach(contact => {
            // create a new element for the contact
            const item = document.createElement("button");
            item.type = "button";
            item.className = "list-group-item list-group-item-action p-3"; // styling

            // contact thumbnail structure
            item.innerHTML = `
                <div class="d-flex w-100 justify-content-between align-items-center">
                    <h6 class="mb-1 fw-bold">${contact.firstName} ${contact.lastName}</h6> 
                    <span class="badge bg-secondary text-uppercase" style="font-size: 0.65rem;">
                        ${getCategoryName(contact.categoryId)}
                    </span>
                </div>
                <small class="text-muted">${contact.email}</small>
            `;

            // add listener to call function showing details after the contact is clicked
            item.addEventListener("click", () => showContactDetails(contact.id));

            // add the contact to the contacts list container
            listContainer.appendChild(item);
        });
    } catch (err) {
        // alternative error message
        listContainer.innerHTML = `<div class="text-center p-3 text-danger">Błąd: ${err.message}</div>`;
    }
}

// Categories data is hardcoded for now, should be fetched from database at the end
const globalCategories = [
    { id: 1, name: "Służbowy" },
    { id: 2, name: "Prywatny" },
    { id: 3, name: "Inny" }
];

const globalSubCategories = [
    { id: 1, categoryId: 1, name: "Szef" },
    { id: 2, categoryId: 1, name: "Klient" },
    { id: 3, categoryId: 1, name: "Pracownik" }
];

function initStaticCategories() {
    const categorySelect = document.getElementById("c-category");
    if (!categorySelect) return;

    categorySelect.innerHTML = '<option value="">Wybierz kategorię</option>';
    globalCategories.forEach(cat => {
        const opt = document.createElement("option");
        opt.value = cat.id;
        opt.textContent = cat.name;
        categorySelect.appendChild(opt);
    });
}

function getCategoryName(id) {
    const found = globalCategories.find(c => c.id == id);
    return found ? found.name : "Inny";
}

// Changing category and filtering subcategories
document.getElementById("c-category").addEventListener("change", (e) => {
    updateSubcategorySelect(e.target.value);
});

function updateSubcategorySelect(categoryId, selectedSubCategoryId = null) {
    const subSelect = document.getElementById("c-subcategory");
    if (!subSelect) return;

    subSelect.innerHTML = '<option value="">-- Wybierz podkategorię --</option>';

    if (!categoryId) {
        subSelect.setAttribute("disabled", "true");
        return;
    }

    const filtered = globalSubCategories.filter(sub => sub.categoryId == categoryId);
    filtered.forEach(sub => {
        const opt = document.createElement("option");
        opt.value = sub.id;
        opt.textContent = sub.name;
        if (selectedSubCategoryId && sub.id == selectedSubCategoryId) {
            opt.selected = true;
        }
        subSelect.appendChild(opt);
    });
}


// Displaying contact details
async function showContactDetails(contactId) {
    document.getElementById("no-selection-alert").classList.add("d-none");
    const detailsCard = document.getElementById("details-card");
    detailsCard.classList.remove("d-none");

    if (document.getElementById("btn-delete-contact")) {
        document.getElementById("btn-delete-contact").classList.remove("d-none");
    }

    try {
        // get contact details from backend
        const response = await fetch(`${API_URL}/contacts/${contactId}`);
        if (!response.ok) throw new Error("Nie udało się pobrać szczegółów kontaktu z bazy.");

        const contact = await response.json();

        document.getElementById("contact-id").value = contact.id;
        document.getElementById("c-firstname").value = contact.firstName;
        document.getElementById("c-lastname").value = contact.lastName;
        document.getElementById("c-email").value = contact.email;
        document.getElementById("c-phone").value = contact.phone || "";
        document.getElementById("c-category").value = contact.categoryId;

        updateSubcategorySelect(contact.categoryId, contact.subcategoryId);

        if (contact.birthDate) {
            document.getElementById("c-birthdate").value = contact.birthDate.split("T")[0];
        } else {
            document.getElementById("c-birthdate").value = "";
        }


        // adjust view based on user's ownership of the contact
        const loggedInUserId = getUserId();
        const actionsSection = document.getElementById("contact-actions");
        const inputs = document.querySelectorAll("#details-card input, #details-card select");

        if (loggedInUserId && contact.userId == loggedInUserId) {
            document.getElementById("details-header").textContent = "Szczegóły twojego kontaktu (możliwa edycja)";
            if (actionsSection) actionsSection.classList.remove("d-none");
            inputs.forEach(input => {
                if (input.id !== "contact-id") input.removeAttribute("disabled");
            });
        } else {
            document.getElementById("details-header").textContent = "Szczegóły kontaktu";
            if (actionsSection) actionsSection.classList.add("d-none");
            inputs.forEach(input => input.setAttribute("disabled", "true"));
        }

    } catch (err) {
        alert(`Błąd: ${err.message}`);
    }
}

// Adding new contact
function initAddContactMode() {
    const addBtn = document.getElementById("btn-add-contact-mode");
    if (!addBtn) return;

    addBtn.addEventListener("click", () => {
        const noSelectionAlert = document.getElementById("no-selection-alert");
        const detailsCard = document.getElementById("details-card");

        if (noSelectionAlert) noSelectionAlert.classList.add("d-none");
        if (detailsCard) detailsCard.classList.remove("d-none");

        document.getElementById("details-header").textContent = "Dodaj nowy kontakt";

        if (document.getElementById("btn-delete-contact")) {
            document.getElementById("btn-delete-contact").classList.add("d-none");
        }

        // clear fields
        document.getElementById("contact-id").value = "";
        document.getElementById("c-firstname").value = "";
        document.getElementById("c-lastname").value = "";
        document.getElementById("c-email").value = "";
        document.getElementById("c-phone").value = "";
        document.getElementById("c-category").value = "";
        document.getElementById("c-birthdate").value = "";

        updateSubcategorySelect("");

        // Odblokowanie pól
        const inputs = document.querySelectorAll("#details-card input, #details-card select");
        inputs.forEach(input => {
            if (input.id !== "contact-id") input.removeAttribute("disabled");
        });

        const actionsSection = document.getElementById("contact-actions");
        if (actionsSection) actionsSection.classList.remove("d-none");
    });
}

// Saving contact (for POST or PUT)
const saveContactBtn = document.getElementById("btn-save-contact");
if (saveContactBtn) {
    saveContactBtn.addEventListener("click", async () => {
        const id = document.getElementById("contact-id").value;

        // method PUT is not available at the frontend site due to lack of time
        if (id) {
            alert("Ze względów czasowych, edycja nie jest powiązana z API. Endpoint (PUT /api/contacts/{id}) dostępny w API.");
            return;
        }

        const subCategoryEl = document.getElementById("c-subcategory");

        const contactData = {
            firstName: document.getElementById("c-firstname").value.trim(),
            lastName: document.getElementById("c-lastname").value.trim(),
            email: document.getElementById("c-email").value.trim(),
            phone: document.getElementById("c-phone").value.trim(),
            categoryId: parseInt(document.getElementById("c-category").value) || null,
            subcategoryId: (subCategoryEl && subCategoryEl.value) ? parseInt(subCategoryEl.value) : null,
            birthDate: document.getElementById("c-birthdate").value || null
        };

        if (!contactData.firstName || !contactData.lastName || !contactData.email) {
            alert("Imię, nazwisko oraz adres e-mail są wymagane!");
            return;
        }

        try {
            const token = getToken();
            const response = await fetch(`${API_URL}/contacts`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(contactData)
            });

            if (response.ok) {
                alert("Kontakt został pomyślnie dodany do bazy danych!");
                loadContacts();

                const detailsCard = document.getElementById("details-card");
                const noSelectionAlert = document.getElementById("no-selection-alert");
                if (detailsCard) detailsCard.classList.add("d-none");
                if (noSelectionAlert) noSelectionAlert.classList.remove("d-none");
            } else {
                const errData = await response.json().catch(() => ({}));
                alert(`Błąd dodawania kontaktu: ${errData.message || "Brak autoryzacji lub niepoprawne dane."}`);
            }
        } catch (err) {
            alert(`Błąd połączenia z API: ${err.message}`);
        }
    });
}

if (document.getElementById("btn-delete-contact")) {
    document.getElementById("btn-delete-contact").addEventListener("click", () => {
        alert("Ze względów czasowych, przycisk nie jest powiązany z API. Endpoint (DELETE /api/contacts/{id}) dostępny do w API.");
    });
}


