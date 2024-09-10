from rest_framework import serializers
from .models import ChatRequest

class ChatRequestSerializer(serializers.ModelSerializer):
    class Meta:
        model = ChatRequest
        fields = ['name',  'response']
