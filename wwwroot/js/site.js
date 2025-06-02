// Warten bis das ganze HTML-Dokument geladen ist
document.addEventListener('DOMContentLoaded', function () {

    // Referenzen auf die wichtigsten HTML-Elemente
    const searchInput = document.getElementById('liveSearch'); // Eingabefeld für die Suche
    const productList = document.getElementById('productList'); // Container für die Produktkarten
    const loader = document.getElementById('loader'); // Spinner, der beim Laden angezeigt wird

    // Prüfen, ob alle benötigten Elemente vorhanden sind
    if (!searchInput || !productList || !loader) return;

    // Speichert den ursprünglichen Inhalt der Produktliste (zum Zurücksetzen bei leerer Suche)
    const originalHtml = productList.innerHTML;

    // Eventlistener: Wird bei jeder Eingabe im Suchfeld ausgelöst
    searchInput.addEventListener('input', function () {
        const query = this.value.trim().toLowerCase(); // Eingabe bereinigen und in Kleinbuchstaben umwandeln

        // Wenn weniger als 2 Zeichen eingegeben wurden → ursprüngliche Liste anzeigen
        if (query.length < 2) {
            productList.innerHTML = originalHtml;
            return;
        }

        // Zeige den Spinner an, während geladen wird
        loader.style.display = 'block';
        productList.innerHTML = ''; // Liste leeren (optional, für besseren visuellen Effekt)

        // AJAX-ähnliche Abfrage mit fetch()
        fetch(`/Products/LiveSearch?query=${encodeURIComponent(query)}`)
            .then(response => {
                // Prüfe, ob die Antwort erfolgreich war (Status 200–299)
                if (!response.ok) throw new Error('Fehler beim Laden');
                return response.json(); // JSON-Daten aus der Antwort extrahieren
            })
            .then(data => {
                // Wenn keine Produkte zurückgegeben wurden
                if (!data || data.length === 0) {
                    productList.innerHTML = '<p class="text-center">Keine Produkte gefunden</p>';
                    return;
                }

                let html = '';

                // Durch alle zurückgegebenen Produkte iterieren und HTML generieren
                data.forEach(product => {
                    html += `
                    <div class="col">
                        <div class="card h-100">
                            <img src="${product.imageUrl}" class="card-img-top" alt="${product.name}" style="height: 200px; object-fit: cover;">
                            <div class="card-body">
                                <h5 class="card-title">${product.name}</h5>
                                <p class="card-text text-muted">${product.description}</p>
                                <p class="card-text fw-bold">
                                    ${product.price.toLocaleString('de-DE', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} €
                                </p>
                                <p class="badge bg-primary">${product.categoryName}</p>
                            </div>
                            <div class="card-footer d-flex justify-content-between align-items-center">
                                ${
                        // Wenn der Benutzer ein Admin ist: Admin-Buttons anzeigen
                        product.isAdmin ? `
                                        <a href="/Products/Details/${product.id}" class="btn btn-sm btn-outline-primary">Details</a>
                                        <a href="/Products/Edit/${product.id}" class="btn btn-sm btn-outline-warning">Bearbeiten</a>
                                        <a href="/Products/Delete/${product.id}" class="btn btn-sm btn-outline-danger">Löschen</a>
                                    ` :
                            // Wenn der Benutzer eingeloggt ist: Warenkorb-Formular anzeigen
                            product.isAuthenticated ? `
                                        <div class="ms-auto">
									        <form class="add-to-cart-form" data-product-id="@item.Id" method="post" action="@Url.Action("AddToCart", "CartItems")">
										        <input type="hidden" name="productId" value="@item.Id" />
										        <button type="submit" class="btn btn-sm btn-success">🛒 In den Warenkorb</button>
									        </form>
								        </div>
                                    ` :
                                // Wenn der Benutzer nicht eingeloggt ist: Login-Hinweis anzeigen
                                `
                                        <a class="btn btn-sm btn-success w-100" href="/Account/Login">
                                            🛒 Jetzt einloggen um zu bestellen
                                        </a>
                                    `
                        }
                            </div>
                        </div>
                    </div>
                    `;
                });

                // Generierten HTML-Code in die Seite einfügen
                productList.innerHTML = html;
            })
            .catch(() => {
                // Falls ein Fehler auftritt, zeige Fehlermeldung
                productList.innerHTML = '<p class="text-danger">Fehler beim Laden der Produkte</p>';
            })
            .finally(() => {
                // Spinner in jedem Fall wieder ausblenden
                loader.style.display = 'none';
            });
    });

    // Event-Delegation: "submit"-Event für dynamisch erzeugte "In den Warenkorb"-Formulare
    productList.addEventListener('submit', function (e) {
        // Prüfen, ob das Event auf einem Warenkorb-Formular ausgelöst wurde
        if (!e.target.classList.contains('add-to-cart-form')) return;

        e.preventDefault(); // Standardverhalten (Seiten-Reload) verhindern

        const form = e.target;
        const productId = form.dataset.productId; // Produkt-ID aus dem data-Attribut

        // POST-Anfrage senden, um das Produkt zum Warenkorb hinzuzufügen
        fetch(form.action, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
            },
            body: `productId=${encodeURIComponent(productId)}`
        })
            .then(response => response.json())
            .then(data => {
                // Wenn die Antwort die Anzahl enthält, aktualisiere die Warenkorb-Anzeige
                if (data.count !== undefined) {
                    document.getElementById('cart-count').textContent = data.count;
                }
            })
            .catch(() => {
                // Fehlerbehandlung, falls POST schiefgeht
                alert('Fehler beim Hinzufügen zum Warenkorb.');
            });
    });
});
