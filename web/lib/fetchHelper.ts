export async function safeJson(res: Response): Promise<any> {
    const text = await res.text();
    try {
        return JSON.parse(text);
    } catch {
        const status = res.status;
        if (status === 429) return { success: false, message: "Zbyt wiele prób — poczekaj chwilę i spróbuj ponownie." };
        if (status >= 500) return { success: false, message: "Błąd serwera — spróbuj ponownie za chwilę." };
        return { success: false, message: text || "Nieoczekiwana odpowiedź serwera." };
    }
}