from google import genai
from google.genai import types
from dotenv import load_dotenv
load_dotenv()
client = genai.Client()

def get_image_description(base64:str,media_type:str,len="PL/pl"):
    response = client.models.generate_content(
        model="gemini-3.5-flash-lite",
        contents=[
            types.Part.from_bytes(data=base64,mime_type=media_type),
            """Opisz zawartość obrazu po {len}, tak aby opis dało się później łatwo wyszukać.
Zasady:
- Napisz 3–4 krótkie, rzeczowe zdania jako jeden akapit, bez numeracji, list i formatowania.
- Zacznij od rodzaju obrazu (np. zdjęcie, zrzut ekranu, skan dokumentu, wykres, rysunek).
- Wymień najważniejsze obiekty, osoby lub zwierzęta, co robią, oraz miejsce i otoczenie.
- Używaj konkretnych, prostych nazw (np. „pies rasy labrador”, „rower górski”, „plaża”), a nie ogólników.
- Podaj istotne cechy: kolory, liczbę obiektów, porę dnia, pogodę, jeśli są wyraźnie widoczne.
- Jeśli na obrazie jest czytelny tekst (napisy, nazwy, numery, logo), przepisz najważniejsze fragmenty dosłownie.
- Opisuj tylko to, co widać. Nie zgaduj, nie interpretuj emocji i nie oceniaj.
- Nie zaczynaj od „Na zdjęciu widać” ani „Obraz przedstawia”."""
        ]
    )
    return response.text
