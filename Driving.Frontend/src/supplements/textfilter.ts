export function textFilter(object: object, text: string): boolean {
  let regex: RegExp;
  try {
    regex = new RegExp(text, "i");
  } catch {
    return false;
  }

  return Object.values(object).some((value) => {
    if (typeof value === "string") {
      return regex.test(value);
    }
    if (Array.isArray(value)) {
      return value.some((v) => textFilter(v, text));
    }
    if (typeof value === "object" && value !== null) {
      return textFilter(value, text);
    }
    return false;
  });
}
