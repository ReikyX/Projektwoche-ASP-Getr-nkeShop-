namespace DrinkShop.Models
{
    // ViewModel f�r die Fehlerseite
    public class ErrorViewModel
    {
        // Optional: Die ID der Anfrage (Request), die zu dem Fehler gef�hrt hat
        public string? RequestId { get; set; }

        // Gibt an, ob die RequestId angezeigt werden soll
        // Wenn RequestId nicht null oder leer ist, wird true zur�ckgegeben
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
