from rest_framework.decorators import api_view
from rest_framework.response import Response
from rest_framework import status
import openai
import random
from transformers import AutoTokenizer, AutoModelForCausalLM,Pipeline
import torch
from collections import defaultdict
import json

# In-memory storage for conversation history
conversation_history = defaultdict(list)



@api_view(['POST'])
def handle_post_symptom_description(request):
    key = request.headers.get('Key')
    token = request.headers.get('Token')
    request_id = request.headers.get('Request-ID')  # New header for request ID

    # Check if Token and Request-ID are valid
    if token != '60190e8e-32e0-4974-8369-956c6852ac1f' or not key or not request_id:
        return Response({'error': 'Invalid API credentials or missing Request-ID'}, status=status.HTTP_401_UNAUTHORIZED)
    
    data = request.data
    symptom_description = data.get('symptom_description')

    if not symptom_description:
        return Response({'error': 'symptom_description field must be provided.'}, status=status.HTTP_400_BAD_REQUEST)

    system_content = """
    You are a health professional chatbot. Your task is to determine if the user's health description is clear and provides sufficient information.

    If the health description is not a valid one respond with:
    "INVALID: "

    If the health description is clear and provides sufficient details, respond with:
    "VALID: [To help healthcare professionals understand your symptoms better, it would be helpful if you could provide more details about symptom provided by the user]"

    If the health description needs more clarification or additional information, respond with:
    "VALID: Q1: [First question] Q2: [Second question] Q3: [Third question] QEND"

    If the user's input is outside the scope of describing personal health issues, respond with:
    "OFFPOINT: The input is not related to health descriptions."

    
    Ensure the analysis is accurate and focused on health-related information.
    Do not provide medical advice or recommend emergency services.
    """

    client = openai.OpenAI(
        api_key=key,
        base_url="https://api.aimlapi.com",
    )

    try:
        # Retrieve conversation history for this request ID
        history = conversation_history.get(request_id, [])

        # Prepare messages including history
        messages = [
            {"role": "system", "content": system_content},
        ]
        messages.extend(history)
        messages.append({"role": "user", "content": symptom_description})

        chat_completion = client.chat.completions.create(
            model="mistralai/Mistral-7B-Instruct-v0.2",
            messages=messages,
            temperature=0.7,
            max_tokens=256,
        )

        response_content = chat_completion.choices[0].message.content
        print(response_content)

        # Update conversation history
        history.append({"role": "user", "content": symptom_description})
        history.append({"role": "assistant", "content": response_content})
        conversation_history[request_id] = history[-10:]  # Keep last 10 messages

        if response_content.startswith("VALID:") or response_content.startswith("NOTVALID:"):
            validation_status, _, message = response_content.partition("\n")
            validation_status = validation_status.split(":")[0].strip()
            return Response({
                'validation_status': validation_status,
                'message': message.strip()
            }, status=status.HTTP_200_OK)
        elif response_content.startswith("OFFPOINT:"):
            return Response({
                'validation_status': 'OFFPOINT',
                'message': response_content
            }, status=status.HTTP_200_OK)
        else:
            return Response({
                'validation_status': 'UNKNOWN',
                'message': response_content
            }, status=status.HTTP_200_OK)

    except Exception as e:
        return Response({'error': str(e)}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)


@api_view(['POST'])
def system_Prompt(request):
    key = request.headers.get('Key')
    token = request.headers.get('Token')
    request_id = request.headers.get('Request-ID')  # New header for request ID

    # Check if Token and Request-ID are valid
    if token != '60190e8e-32e0-4974-8369-956c6852ac1f' or not key or not request_id:
        return Response({'error': 'Invalid API credentials or missing Request-ID'}, status=status.HTTP_401_UNAUTHORIZED)
    
    data = request.data
    userInput = data.get('inputText')
    system_content = data.get('system_content')

    if not userInput:
        return Response({'error': 'userInput field must be provided.'}, status=status.HTTP_400_BAD_REQUEST)
    
    if not system_content:
        return Response({'error': 'system_content field must be provided.'}, status=status.HTTP_400_BAD_REQUEST)

    client = openai.OpenAI(
        api_key=key,
        base_url="https://api.aimlapi.com",
    )

    try:
        # Retrieve conversation history for this request ID
        history = conversation_history.get(request_id, [])

        # Prepare messages including history
        messages = [
            {"role": "system", "content": system_content},
        ]
        messages.extend(history)
        messages.append({"role": "user", "content": userInput})

        chat_completion = client.chat.completions.create(
            model="mistralai/Mistral-7B-Instruct-v0.2",
            messages=messages,
            temperature=0.7,
            max_tokens=256,
        )

        response_content = chat_completion.choices[0].message.content.strip()
        print(response_content)

        # Update conversation history
        history.append({"role": "user", "content": userInput})
        history.append({"role": "assistant", "content": response_content})
        conversation_history[request_id] = history[-10:]  # Keep last 10 messages

        return Response({
            'message': response_content
        }, status=status.HTTP_200_OK)

    except Exception as e:
        return Response({'error': str(e)}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)
    
    
@api_view(['POST'])
def handle_post_analyse_health_Condition(request):
    key = request.headers.get('Key')
    token = request.headers.get('Token')
    request_id = request.headers.get('Request-ID')  # New header for request ID

    # Check if Token and Request-ID are valid
    if token != '60190e8e-32e0-4974-8369-956c6852ac1f' or not key or not request_id:
        return Response({'error': 'Invalid API credentials or missing Request-ID'}, status=status.HTTP_401_UNAUTHORIZED)
    
    data = request.data
    questions_and_answers = data.get('questions_and_answers')
    health_condition = data.get('health_condition')  # Get the health condition from the request data

    if not questions_and_answers:
        return Response({'error': 'questions_and_answers field must be provided.'}, status=status.HTTP_400_BAD_REQUEST)

    if not health_condition:
        return Response({'error': 'health_condition field must be provided.'}, status=status.HTTP_400_BAD_REQUEST)

    system_content_template = """
    You are a health professional chatbot. Your task is to  reveiew questions and answer provided for health condition by a patient and summerize the questions and the health in a way that a doctor
     will understand it using second person singluar .

    Kindly review all the questions and answers and see if the question is sufficient for the answer in regard to the health condition: {health_condition}

    If the health description is not a valid one respond with:
    "INVALID: followed by the inofrmation provided is not sufficient"

    If the health description is clear and provides sufficient details, respond with:
    "VALID: following the Summary of the health condition following the question and answers the patient provided"

    Ensure the analysis is accurate and focused on health-related information.
    Do not provide medical advice or recommend emergency services.
    """

    system_content = system_content_template.format(health_condition=health_condition)

    client = openai.OpenAI(
        api_key=key,
        base_url="https://api.aimlapi.com",
    )

    try:
        # Retrieve conversation history for this request ID
        history = conversation_history.get(request_id, [])

        # Prepare messages including history
        messages = [
            {"role": "system", "content": system_content},
        ]
        messages.extend(history)
        messages.append({"role": "user", "content": json.dumps(questions_and_answers)})

        chat_completion = client.chat.completions.create(
            model="mistralai/Mistral-7B-Instruct-v0.2",
            messages=messages,
            temperature=0.7,
            max_tokens=256,
        )

        response_content = chat_completion.choices[0].message.content.strip()
        print(response_content)

        # Update conversation history
        history.append({"role": "user", "content": json.dumps(questions_and_answers)})
        history.append({"role": "assistant", "content": response_content})
        conversation_history[request_id] = history[-10:]  # Keep last 10 messages

        if response_content.startswith("VALID") or response_content.startswith("INVALID"):
            validation_status = response_content.split(":")[0].strip()
            summary = response_content[len(validation_status)+1:].strip()
            return Response({
                'validation_status': validation_status,
                'message': response_content
            }, status=status.HTTP_200_OK)
        else:
            return Response({
                'validation_status': 'INVALID',
                'message': response_content
            }, status=status.HTTP_200_OK)

    except Exception as e:
        return Response({'error': str(e)}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)
    
@api_view(['POST'])
def handle_post_Gpt2(request):
    token = request.headers.get('Token')
    request_id = request.headers.get('Request-ID')  # New header for request ID

    # Check if Token and Request-ID are valid
    if token != '60190e8e-32e0-4974-8369-956c6852ac1f' or not request_id:
        return Response({'error': 'Invalid API credentials or missing Request-ID'}, status=status.HTTP_401_UNAUTHORIZED)
    
    data = request.data
    user_message = data.get('message')
    system_content = data.get('system_content')

    if not system_content:
        return Response({'error': 'system_content field must be provided.'}, status=status.HTTP_400_BAD_REQUEST)

    if not user_message:
        return Response({'error': 'message field must be provided.'}, status=status.HTTP_400_BAD_REQUEST)

    try:
        # Load the model and tokenizer
        model_name = "reginald160/gpt2-Ozougwu-Model-B2"
        tokenizer = AutoTokenizer.from_pretrained(model_name)
        model = AutoModelForCausalLM.from_pretrained(model_name)

        # Retrieve conversation history for this request ID
        history = conversation_history.get(request_id, [])

        # Prepare input including history
        input_text = f"{system_content}\n\n"
        for msg in history:
            if msg["role"] == "user":
                input_text += f"User: {msg['content']}\n"
            elif msg["role"] == "assistant":
                input_text += f"AI: {msg['content']}\n"
        input_text += f"User: {user_message}\nAI:"

        inputs = tokenizer(input_text, return_tensors="pt", truncation=True, max_length=512, padding=True)

        # Generate response
        output = model.generate(
            inputs['input_ids'],
            attention_mask=inputs['attention_mask'],
            max_new_tokens=200,
            num_return_sequences=1,
            no_repeat_ngram_size=2,
            pad_token_id=tokenizer.eos_token_id,
            do_sample=True,
            temperature=0.7
        )
        
        response_content = tokenizer.decode(output[0], skip_special_tokens=True)

        # Extract the AI's response
        ai_response = response_content.split("AI:")[-1].strip()

        print(ai_response)

        # Update conversation history
        history.append({"role": "user", "content": user_message})
        history.append({"role": "assistant", "content": ai_response})
        conversation_history[request_id] = history[-10:]  # Keep last 10 messages

        return Response({
            'message': response_content
        }, status=status.HTTP_200_OK)

    except Exception as e:
        return Response({'error': str(e)}, status=status.HTTP_500_INTERNAL_SERVER_ERROR)

