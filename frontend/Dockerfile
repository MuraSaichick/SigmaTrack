FROM node:24-alpine AS builder 
WORKDIR /app

COPY package.json ./

RUN npm install

COPY . .

RUN npm run build

FROM node:24-alpine AS runner
WORKDIR /app

COPY --from=builder /app/.output ./.output

ENV HOST=0.0.0.0
ENV PORT=3000
ENV NODE_ENV=production

EXPOSE 3000

CMD ["node", ".output/server/index.mjs"]