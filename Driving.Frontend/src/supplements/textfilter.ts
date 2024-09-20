export function textFilter(object: object, text: string): boolean {
  return Object.values(object).some((value) => {
    if (typeof value === "string") {
      return value.toLowerCase().includes(text.toLowerCase());
    }
    if (Array.isArray(value)) {
      return value.some((v) => textFilter(v, text));
    }
    return false;
  });
}
