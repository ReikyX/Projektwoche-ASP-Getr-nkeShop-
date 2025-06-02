namespace DrinkShop.Models
{
    // ViewModel für die Fehlerseite
    public class ErrorViewModel
    {
        // Optional: Die ID der Anfrage (Request), die zu dem Fehler geführt hat
        public string? RequestId { get; set; }

        // Gibt an, ob die RequestId angezeigt werden soll
        // Wenn RequestId nicht null oder leer ist, wird true zurückgegeben
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
