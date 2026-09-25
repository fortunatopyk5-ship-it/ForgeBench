# Proof attempts

## Attempt A — spectral extraction from Euclidean-time decay
Result: successful conditional bridge (YM-BRIDGE-1).

Core identity:
C_O(t)=<psi_O,e^{-tH}psi_O>=integral_[0,infinity) e^{-tE} dmu_O(E).

A common decay rate m_* on a dense centered local algebra rules out spectral measure below m_*.

Important restriction:
The argument requires Euclidean **time** translation after reflection positivity/OS reconstruction. Generic spatial covariance decay is not automatically a spectral theorem unless Euclidean invariance lets it be placed in the reconstructed time direction.

## Attempt B — functional inequality plus locality
At strong coupling, the Shen–R. Zhu–X. Zhu proof decomposes covariance into a semigroup decorrelation term controlled by a Poincare inequality and a quasi-locality term controlled by commutators of local derivatives with the Langevin generator.

Abstract target:
Poincare rate lambda_a + derivative propagation scale v_a
=> covariance exponent gamma_a >= c lambda_a/v_a
=> require lambda_a/v_a >= c' a Lambda_YM.

New observation:
lambda_a alone is not a physically meaningful target because stochastic-quantization time can be rescaled. The ratio lambda_a/v_a is invariant under an overall rescaling of the auxiliary Markov generator and is therefore a better quantity to compare to a spatial correlation length.

Status:
PROMISING ROUTE — NOT A PROOF.

## Attempt C — compare a generic exp(-C beta) lattice bound to asymptotic scaling
For SU(N), beta_W=2N/g_0^2 and
log(a Lambda_YM) = - beta_W/(4N b_0) + O(log beta_W)
with b_0=11N/(48 pi^2).

Therefore a lower bound
gamma(beta_W) >= exp(-C beta_W+O(log beta_W))
only gives a positive continuum lower bound if its exponent/power corrections are at least as strong as the RG scale. If C>1/(4N b_0), the inferred physical lower bound gamma/a tends to zero exponentially and is useless for the Millennium gap.

Status:
PROVED as a comparison criterion, conditional on the standard asymptotic-scaling relation being the chosen trajectory.
