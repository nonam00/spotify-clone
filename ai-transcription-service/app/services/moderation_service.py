from better_profanity import profanity

def check_text_for_explicit_content(text: str) -> bool:
    return profanity.contains_profanity(text)
