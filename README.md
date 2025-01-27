# Text To Speech
A simple .NET Core Project using OpenAI and Firebase for Statistics.

### DEMO LINK! => [Text To Speech](https://insooeric.github.io/#/playground)
---

## Tables of Contents

1. [Overview](#overview)
2. [Text To Speech](#text-to-speech)
3. [Speech To Text](#speech-to-text)
4. [Statistics](#statistics)

---

## Overview
This project is built in .NET Core using several API endpoints:
- `/api/tts-ai` endpoint for text to speech
- `/api/global-statistic` endpoint for overall statistics
- `/api/language-statistic` endpoint for statistics in selected language

---

## Text To Speech
Endpoint: `/api/tts-ai`

### Limitation
- Both input text and voice type is required.
- Input text should never exceed 40 words.

### Request Body

```jsonc
{
    "Input": "Hello, My name Is Insoo Son. I'm from South Korea.",
    "Voice": "alloy"
}
```

### Response

**On success**
- It returns an audio file named `speech.mp3` in Blob type.

**On failiar**
```jsonc
{
    "Message": "Input text is required."
}
```

```jsonc
{
    "Message": "Input text exceeds the 40-word limit. Please shorten your text."
}
```

```jsonc
{
    "Message": "OpenAI API key is missing or not configured."
}
```

There may be an error caused by in sufficient credit. (meaning I would have to charge for it)

---

## Speech To Text
**Note** that end point is not configured; however, I am using it for statistics purpose.
`Middlewares/AudioAnalyzer.cs` includes a code that performs API call for speech to text.

---

## Statistics

### Global Statistic
Endpoint: `/api/global-statistic`

#### Request Body
- Not required

#### Response

**On success**
```jsonc
{
   "message":"Successfully retrieved global statistics.",
   "data":{
      "globalTotalRequests":74,
      "globalTotalDuration":179.09999895095825,
      "globalTotalAverageDuration":2.4202702560940303,
      "globalTotalSentences":84,
      "globalAverageDurationPerSentences":2.1321428446542647,
      "globalToneTypeCounts":{
         "alloy":40,
         "echo":6,
         "fable":2,
         "nova":16,
         "onyx":8,
         "shimmer":2
      }
   }
}
```

**On failiar**
```jsonc
{
  "message": "Error communicating with Firebase.",
  "details": "<Specific error goes here>"
}
```

```jsonc
{
  "message": "An unexpected error occurred.",
  "details": "<Specific error goes here>"
}
```


### Language Statistic
Endpoint: `/api/language-statistic`

#### Request Body
- Not required

#### Response

**On success**
```jsonc
{
   "message":"Successfully retrieved language statistics.",
   "data":{
      "languages":{
         "english":{
            "langTotalRequest":45,
            "langTotalDuration":103.24999868869781,
            "langAverageDuration":2.2944444153043957,
            "langTotalSentences":52,
            "langAverageDurationPerSentences":1.9855768978595734,
            "langToneTypeCounts":{
               "alloy":27,
               "echo":6,
               "fable":1,
               "nova":8,
               "onyx":1,
               "shimmer":2
            }
         }
         "korean":{
            "langTotalRequest":26,
            "langTotalDuration":66.4300000667572,
            "langAverageDuration":2.5550000025675845,
            "langTotalSentences":29,
            "langAverageDurationPerSentences":2.2906896574743865,
            "langToneTypeCounts":{
               "alloy":12,
               "fable":1,
               "nova":7,
               "onyx":6
            }
         },
        ...
      }
   }
}
```


**On failiar**
```jsonc
{
  "message": "Error communicating with Firebase.",
  "details": "<Specific error goes here>"
}
```

```jsonc
{
  "message": "An unexpected error occurred.",
  "details": "<Specific error goes here>"
}
```
