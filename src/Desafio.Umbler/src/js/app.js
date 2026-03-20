import React, { useState } from 'react'
import ReactDOM from 'react-dom/client'

function DomainSearch() {
  const [domain, setDomain] = useState('')
  const [result, setResult] = useState(null)
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)

  const validate = (value) => {
    if (!value || !value.trim()) return 'Digite um domínio.'
    if (!value.includes('.')) return 'Domínio inválido. Ex: umbler.com'
    if (value.startsWith('.') || value.endsWith('.')) return 'Domínio inválido.'
    return null
  }

  const handleSearch = async () => {
    const validationError = validate(domain)
    if (validationError) {
      setError(validationError)
      setResult(null)
      return
    }

    setError('')
    setLoading(true)
    setResult(null)

    try {
      const resp = await fetch(`/api/domain/${domain.trim().toLowerCase()}`, {
        headers: { 'Accept': 'application/json' }
      })

      if (!resp.ok) {
        const msg = await resp.text()
        setError(msg || 'Erro ao consultar domínio.')
        return
      }

      const data = await resp.json()
      setResult(data)
    } catch (e) {
      setError('Erro de conexão.')
    } finally {
      setLoading(false)
    }
  }

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') handleSearch()
  }

  return (
    <div className="py-4">
      <div className="input-group">
        <input
          className="form-control form-control-lg"
          placeholder="Digite o domínio que deseja pesquisar..."
          type="text"
          value={domain}
          onChange={(e) => setDomain(e.target.value)}
          onKeyDown={handleKeyDown}
        />
        <div className="input-group-btn">
          <button
            className="btn btn-success btn-lg"
            onClick={handleSearch}
            disabled={loading}
          >
            {loading ? 'Pesquisando...' : 'Pesquisar'}
          </button>
        </div>
      </div>

      {error && (
        <div className="alert alert-danger mt-2">{error}</div>
      )}

      {result && (
        <div className="card mt-4">
          <div className="card-header">
            <h5 className="card-title mb-0">
              Resultado para <strong className="text-primary">{result.name}</strong>
            </h5>
          </div>
          <div className="list-group list-group-flush">
            <div className="list-group-item">
              <div className="row">
                <div className="col-md-3">
                  <small className="text-muted text-uppercase font-weight-bold">IP Registro A</small>
                </div>
                <div className="col-md-9">
                  <span className="tag tag-primary tag-pill">{result.ip}</span>
                </div>
              </div>
            </div>
            <div className="list-group-item">
              <div className="row">
                <div className="col-md-3">
                  <small className="text-muted text-uppercase font-weight-bold">Hospedado em</small>
                </div>
                <div className="col-md-9">
                  <strong>{result.hostedAt}</strong>
                </div>
              </div>
            </div>
            <div className="list-group-item">
              <div className="row">
                <div className="col-md-3">
                  <small className="text-muted text-uppercase font-weight-bold">Name Servers</small>
                </div>
                <div className="col-md-9">
                  {result.nameServers && result.nameServers.map((ns, i) => (
                    <span key={i} className="tag tag-default tag-pill mr-1">{ns}</span>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}

const root = ReactDOM.createRoot(document.getElementById('whois-results'))
root.render(<DomainSearch />)