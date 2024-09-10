from django.db import models

class ChatRequest(models.Model):
    name = models.CharField(max_length=255)
    id = models.BigAutoField(primary_key=True)
    response = models.TextField(blank=True, null=True)

    def __str__(self):
        return self.name
