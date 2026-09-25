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


## Attempt D — abstract semigroup-to-space theorem
Result: YM-SEMIGROUP-1 proved under explicit assumptions.

The key rate is not the Poincare constant lambda_a by itself but
  rho_a(kappa)=lambda_a/(lambda_a+v_{kappa,a}),
where v_{kappa,a} controls how quickly a local derivative can spread under the auxiliary semigroup.

For a reflection-positive lattice theory this gives the sufficient physical-gap estimate
  Delta_a >= [kappa/(2a)] rho_a(kappa).

This ratio is invariant under an overall rescaling of stochastic-quantization time L_a -> r L_a, because both lambda_a and v_{kappa,a} scale by r.

Adversarial correction:
A global PI may be stronger than necessary. Slow global/topological modes can make an auxiliary Markov gap tiny while local Euclidean correlations remain short-ranged. Future work should replace global PI by conditional/block or local-observable relaxation.

## Attempt E — RG landing into a polymer mixing domain
Instead of analytically continuing the strong-coupling proof in the bare beta, run an exact RG from weak UV coupling toward the IR.

Concrete sufficient landing condition:
at a coarse spacing a_*~Lambda_YM^{-1}, the effective polymer activities satisfy a cutoff-independent Kotecky-Preiss norm bound with exponential diameter weight.

If this single landing estimate is proved, standard cluster expansion gives exponential correlation decay at rate O(1/a_*)=O(Lambda_YM), and original-lattice reflection positivity gives the physical mass gap.

Status:
PROMISING ROUTE — NOT A PROOF.
